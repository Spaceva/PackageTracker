using GitLabApiClient;
using GitLabApiClient.Models.Branches.Responses;
using GitLabApiClient.Models.Projects.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Json;
using static PackageTracker.Scanner.ScannerSettings;
using File = GitLabApiClient.Models.Files.Responses.File;

namespace PackageTracker.Scanner.Gitlab;

internal class GitlabScanner(TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IMediator mediator)
    : BaseScanner<Project>(trackedApplication, moduleParsers, logger, mediator)
{
    private static readonly TimeSpan DefaultTokenExpirationWarningThreshold = TimeSpan.FromDays(7);

    private TimeSpan TokenExpirationWarningThreshold => TrackedApplication.TokenExpirationWarningThreshold ?? DefaultTokenExpirationWarningThreshold;

    private GitLabClient? gitLabClient;
    private GitLabClient GitLabClient
    {
        get
        {
            gitLabClient ??= new(TrackedApplication.RepositoryRootLink, TrackedApplication.AccessToken);
            return gitLabClient;
        }
    }

    private HttpClient? httpClient;
    private HttpClient HttpClient
    {
        get
        {
            if (httpClient is null)
            {
                httpClient = new() { BaseAddress = new Uri(TrackedApplication.RepositoryRootLink), };
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + TrackedApplication.AccessToken);
            }

            return httpClient;
        }
    }

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
            await Parallel.ForEachAsync(projects.Where<Project>(p => !p.Archived), cancellationToken, async (project, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    Logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", RepositoryType.Gitlab, project.Name);
                    var application = await ScanProjectAsync(project, cancellationToken);
                    if (application is not null)
                    {
                        applications.Add(application);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Scan failed for {ScannerType} Repository '{RepositoryName}' : {ExceptionMessage}", RepositoryType.Gitlab, project.Name, ex.Message);
                    throw;
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

    protected override RepositoryType RepositoryType => RepositoryType.Gitlab;

    protected override UntypedApplication AsUntypedApplication(Project repository)
    => new() { Name = repository.Name, Path = repository.Namespace.FullPath.Replace("/", ">"), RepositoryLink = repository.HttpUrlToRepo.Replace(".git", "/"), Branchs = [], RepositoryType = RepositoryType.Gitlab };

    protected override string BranchLinkSuffix(string branchName) => $"-/tree/{branchName}";

    protected override void Dispose(bool disposing)
    {
        HttpClient.Dispose();
    }

    protected override async Task<IEnumerable<Project>> GetRepositoriesAsync(CancellationToken cancellationToken) => await GitLabClient.Projects.GetAsync(opt => { });

    protected override bool IsNotArchived(Project repository) => !repository.Archived;

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

                applicationBranchs.Add(ApplicationBranch.From(branch.Name, project.HttpUrlToRepo.Replace(".git", "/") + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application.From(project.Name, project.Namespace.FullPath.Replace("/", ">"), project.HttpUrlToRepo.Replace(".git", "/"), applicationBranchs, RepositoryType.Gitlab);
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
            Logger.LogWarning("Application {ApplicationName} skipped because of Gitlab Error : {ExceptionMessage}.", project.Name, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", project.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private void CheckTokenExpirationWarning(TimeSpan tokenLifeSpan)
    {
        if (tokenLifeSpan < TokenExpirationWarningThreshold)
        {
            Logger.LogWarning("Token is expiring soon : {TokenLifeSpan}", tokenLifeSpan.ToString(@"dd\:hh\:mm\:ss"));
        }
    }

    private Task<PersonnalAccessToken?> TokenInfo(CancellationToken cancellationToken)
        => HttpClient.GetFromJsonAsync<PersonnalAccessToken>("api/v4/personal_access_tokens/self", cancellationToken);

    private async Task<IReadOnlyCollection<File>> FindModuleFiles(int projectId, string branchName)
    {
        var tree = await GitLabClient.Trees.GetAsync(projectId, o => { o.Recursive = true; o.Reference = branchName; });
        var fileHeaders = tree.Where(f => ModuleParsers.Any(p => p.IsModuleFile(f.Path)));
        var filesTask = fileHeaders.Select(fh => GitLabClient.Files.GetAsync(projectId, fh.Path, branchName));
        return await Task.WhenAll(filesTask);
    }

    private async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(int projectId)
    {
        var branches = await GitLabClient.Branches.GetAsync(projectId, o => { });
        var branchsNames = branches.Select(b => b.Name).Intersect(Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<ApplicationModule>> ScanBranchAsync(Project project, string branchName, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(project.Id, branchName);
        if (moduleFiles.Count == 0)
        {
            return [];
        }

        var modules = new List<ApplicationModule>();
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

    private async Task<ApplicationModule?> ScanModuleAsync(File moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = ModuleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.ContentDecoded));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.ContentDecoded, moduleFile.Filename, cancellationToken);
    }
}
