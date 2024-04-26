using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using System.Collections.Concurrent;
using Application = PackageTracker.Domain.Application.Model.Application;

namespace PackageTracker.Scanner.GitHub;

internal abstract class GitHubScanner<TApplicationModule>(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, ScannerSettings.TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<TApplicationModule>> moduleParsers, ILogger logger)
    : GitHubScanner(getRepositoriesDelegate, trackedApplication, mediator, logger)
    where TApplicationModule : ApplicationModule
{
    private protected IEnumerable<IApplicationModuleParser<TApplicationModule>> ModuleParsers => moduleParsers;

    public override async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var rateLimits = await GitHubClient.RateLimit.GetRateLimits();
        if (rateLimits.Rate.Remaining == 0)
        {
            throw new ApiException("Rate limit exceeded", System.Net.HttpStatusCode.TooManyRequests);
        }
        else if ((double)rateLimits.Rate.Remaining / rateLimits.Rate.Limit < 0.15d)
        {
            Logger.LogWarning("Rate limit almost reached: {Remaining} remaining.", rateLimits.Rate.Remaining);
        }

        var applications = new ConcurrentBag<Application>();
        var repositories = await GetRepositories(GitHubClient, OrganizationOrUserName);
        using var semaphore = new SemaphoreSlim(MaximumConcurrencyCalls, MaximumConcurrencyCalls);
        try
        {
            await Parallel.ForEachAsync(repositories, cancellationToken, async (repository, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    Logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", Domain.Application.Model.RepositoryType.GitHub, repository.Name);
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
                var modules = await ScanBranchAsync(repository, branch, cancellationToken);
                if (!modules.Any())
                {
                    continue;
                }

                var commitSha = branch.Commit.Sha;
                var commitInfo = await GitHubClient.Git.Commit.Get(repository.Id, commitSha);
                var lastCommitDate = commitInfo.Committer.Date;

                applicationBranchs.Add(ApplicationBranch(branch.Name, repository.HtmlUrl + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application(repository.Name, repository.FullName.Replace("/", ">").Replace($">{repository.Name}", string.Empty), repository.HtmlUrl, applicationBranchs);
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
    => $"/tree/{branchName}";

    private protected abstract ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit);

    private protected async Task<IReadOnlyCollection<RepositoryContent>> FindModuleFiles(Repository repository, Branch branch)
    {
        var treeResponse = await GitHubClient.Git.Tree.GetRecursive(repository.Id, branch.Commit.Sha);
        var fileHeaders = treeResponse.Tree.Where(TreeItemMatchPattern);
        var filesTask = fileHeaders.Select(fh => GitHubClient.Repository.Content.GetAllContentsByRef(repository.Owner.Login, repository.Name, fh.Path, branch.Commit.Sha));
        var content = await Task.WhenAll(filesTask);
        return [.. content.Select(a => a[0])];
    }

    protected abstract bool TreeItemMatchPattern(TreeItem item);

    private protected async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(long repositoryId)
    {
        var branches = await GitHubClient.Repository.Branch.GetAll(repositoryId);
        var branchsNames = branches.Select(b => b.Name).Intersect(Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<TApplicationModule>> ScanBranchAsync(Repository repository, Branch branch, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(repository, branch);
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
                Logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Name, branch.Name, repository.Name, ex.GetType().Name, ex.Message);
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
