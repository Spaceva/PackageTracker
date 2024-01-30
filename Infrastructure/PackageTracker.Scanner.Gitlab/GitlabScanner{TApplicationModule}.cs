using GitLabApiClient;
using GitLabApiClient.Models.Branches.Responses;
using GitLabApiClient.Models.Projects.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Scanner.Gitlab;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Json;
using static PackageTracker.Scanner.ScannerSettings;
using File = GitLabApiClient.Models.Files.Responses.File;

namespace PackageTracker.Scanner.Gitlab;

internal abstract class GitlabScanner<TApplicationModule> : GitlabScanner where TApplicationModule : ApplicationModule
{
    protected GitlabScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<TApplicationModule>> moduleParsers, ILogger logger)
        : base(trackedApplication, mediator, logger)
    {
        ModuleParsers = moduleParsers;
    }

    private protected IEnumerable<IApplicationModuleParser<TApplicationModule>> ModuleParsers { get; }

    public override async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var tokenInfo = await TokenInfo(cancellationToken);
        if (tokenInfo is not { Active: true, Revoked: false })
        {
            throw new GitLabException(System.Net.HttpStatusCode.Unauthorized, "Token is inactive or revoked.");
        }

        var tokenLifeSpan = tokenInfo.ExpiresAt.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow;
        CheckTokenExpirationWarning(tokenLifeSpan);

        var applications = new ConcurrentBag<Application>();
        var projects = await GitLabClient.Projects.GetAsync(opt => { });
        using var semaphore = new SemaphoreSlim(MaximumConcurrencyCalls, MaximumConcurrencyCalls);
        try
        {
            await Parallel.ForEachAsync(projects.Where<Project>(p => !p.Archived), cancellationToken, (Func<Project, CancellationToken, ValueTask>)(async (project, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    var application = await ScanProjectAsync(project, cancellationToken);
                    if(application is not null)
                    {
                        applications.Add(application);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }));
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

    private async Task<Application?> ScanProjectAsync(Project project, CancellationToken cancellationToken)
    {
        try
        {
            var branchs = await FindAllLongTermBranchs(project.Id);

            var applicationBranchs = new List<ApplicationBranch>();
            foreach (var branch in branchs)
            {
                var modules = await ScanBranchAsync(project, branch.Name, cancellationToken);
                if (!modules.Any())
                {
                    continue;
                }

                var lastCommitDate = DateTimeOffset.Parse(branch.Commit.CommittedDate, CultureInfo.CurrentCulture);

                applicationBranchs.Add(ApplicationBranch(branch.Name, project.HttpUrlToRepo.Replace(".git", "/") + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application(project.Name, project.Namespace.FullPath.Replace("/", ">"), project.HttpUrlToRepo.Replace(".git", "/"), applicationBranchs);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (GitLabException ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of Gitlab Error : {ExceptionMessage}.", project.Name,


















































































                ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", project.Name, ex.GetType().Name, ex.Message);
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

    private async Task<PersonnalAccessToken?> TokenInfo(CancellationToken cancellationToken)
    {
        return await HttpClient.GetFromJsonAsync<PersonnalAccessToken>("api/v4/personal_access_tokens/self", cancellationToken);
    }

    private protected abstract ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit);

    private protected abstract Task<IReadOnlyCollection<File>> FindModuleFiles(int projectId, string branchName);

    private protected async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(int projectId)
    {
        var branches = await GitLabClient.Branches.GetAsync(projectId, o => { });
        var branchsNames = branches.Select(b => b.Name).Intersect(Scanner.Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<TApplicationModule>> ScanBranchAsync(Project project, string branchName, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(project.Id, branchName);
        if (moduleFiles.Count == 0)
        {
            return Array.Empty<TApplicationModule>();
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
                Logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Filename, branchName, project.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<TApplicationModule?> ScanModuleAsync(File moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = ModuleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.ContentDecoded));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.ContentDecoded, moduleFile.Filename, cancellationToken);
    }
}
