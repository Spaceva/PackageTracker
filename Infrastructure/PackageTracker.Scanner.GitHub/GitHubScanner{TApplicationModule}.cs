using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using System.Collections.Concurrent;
using static PackageTracker.Scanner.ScannerSettings;
using Application = PackageTracker.Domain.Application.Model.Application;

namespace PackageTracker.Scanner.GitHub;

internal abstract class GitHubScanner<TApplicationModule> : GitHubScanner where TApplicationModule : ApplicationModule
{
    protected GitHubScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<TApplicationModule>> moduleParsers, ILogger logger)
        : base(trackedApplication, mediator, logger)
    {
        ModuleParsers = moduleParsers;
    }

    private protected IEnumerable<IApplicationModuleParser<TApplicationModule>> ModuleParsers { get; }

    public override async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var rateLimits = await GitHubClient.RateLimit.GetRateLimits();
        if (rateLimits.Rate.Remaining == 0)
        {
            throw new ApiException("Rate limit exceeded", System.Net.HttpStatusCode.TooManyRequests);
        }

        var applications = new ConcurrentBag<Application>();
        var repositories = await GitHubClient.Repository.GetAllForOrg(OrganizationName);
        using var semaphore = new SemaphoreSlim(MaximumConcurrencyCalls, MaximumConcurrencyCalls);
        try
        {
            await Parallel.ForEachAsync(repositories.Where(p => !p.Archived), cancellationToken, async (repository, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    var application = await ScanRepositoryAsync(repository, cancellationToken);
                    if (application is not null)
                    {
                        applications.Add(application);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
            return [];
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
            return [];
        }

        return applications;
    }

    private async Task<Application?> ScanRepositoryAsync(Repository repository, CancellationToken cancellationToken)
    {
        try
        {
            var branchs = await FindAllLongTermBranchs(repository.Id);

            var applicationBranchs = new List<ApplicationBranch>();
            foreach (var branch in branchs)
            {
                var modules = await ScanBranchAsync(repository, branch.Name, cancellationToken);
                if (!modules.Any())
                {
                    continue;
                }

                var commitRef = branch.Commit.Ref;
                var commitInfo = await this.GitHubClient.Git.Commit.Get(repository.Id, commitRef);
                var lastCommitDate = commitInfo.Committer.Date;

                applicationBranchs.Add(ApplicationBranch(branch.Name, repository.Url + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application(repository.Name, repository.FullName.Replace("/", ">"), repository.Url, applicationBranchs);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of GitHub Error : {ExceptionMessage}.", repository.Name, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private static string BranchLinkSuffix(string branchName)
    => $"-/tree/{branchName}";

    private void CheckTokenExpirationWarning(TimeSpan tokenLifeSpan)
    {
        if (tokenLifeSpan < TokenExpirationWarningThreshold)
        {
            Logger.LogWarning("Token is expiring soon : {TokenLifeSpan}", tokenLifeSpan.ToString(@"dd\:hh\:mm\:ss"));
        }
    }

    private protected abstract ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit);

    private protected abstract Task<IReadOnlyCollection<RepositoryContent>> FindModuleFiles(long projectId, string branchName);

    private protected async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(long repositoryId)
    {
        var branches = await this.GitHubClient.Repository.Branch.GetAll(repositoryId);
        var branchsNames = branches.Select(b => b.Name).Intersect(Scanner.Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<TApplicationModule>> ScanBranchAsync(Repository repository, string branchName, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(repository.Id, branchName);
        if (moduleFiles.Count == 0)
        {
            return [];
        }

        var modules = new List<TApplicationModule>();
        foreach (var moduleFile in moduleFiles)
        {
            try
            {
                var module = await ScanModuleAsync(moduleFile, cancellationToken);
                if (module is null)
                {
                    continue;
                }

                modules.Add(module);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Name, branchName, repository.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<TApplicationModule?> ScanModuleAsync(RepositoryContent moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = ModuleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.Content));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.Content, moduleFile.Name, cancellationToken);
    }
}
