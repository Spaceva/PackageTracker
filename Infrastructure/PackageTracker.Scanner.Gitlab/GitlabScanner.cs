using GitLabApiClient;
using GitLabApiClient.Models.Branches.Responses;
using GitLabApiClient.Models.Projects.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http.Json;
using static PackageTracker.Scanner.ScannerSettings;
using File = GitLabApiClient.Models.Files.Responses.File;

namespace PackageTracker.Scanner.Gitlab;

internal class GitlabScanner(TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, IMediator mediator, ILogger logger) : IApplicationsScanner
{
    private static readonly TimeSpan DefaultTokenExpirationWarningThreshold = TimeSpan.FromDays(7);

    private readonly GitLabClient gitLabClient = new(trackedApplication.RepositoryRootLink, trackedApplication.AccessToken);

    private readonly TimeSpan tokenExpirationWarningThreshold = trackedApplication.TokenExpirationWarningThreshold ?? DefaultTokenExpirationWarningThreshold;

    private readonly int maximumConcurrencyCalls = trackedApplication.MaximumConcurrencyCalls;

    private HttpClient? httpClient;
    private HttpClient HttpClient
    {
        get
        {
            if (httpClient is null)
            {
                httpClient = new() { BaseAddress = new Uri(trackedApplication.RepositoryRootLink), };
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + trackedApplication.AccessToken);
            }

            return httpClient;
        }
    }

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { RepositoryTypes = [RepositoryType.Gitlab], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var projects = await gitLabClient.Projects.GetAsync(opt => { });
        var remoteApplications = projects.Where(p => !p.Archived).Select(project => new UntypedApplication { Name = project.Name, Path = project.Namespace.FullPath.Replace("/", ">"), RepositoryLink = project.HttpUrlToRepo.Replace(".git", "/"), Branchs = [] }).ToArray();

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    public async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var tokenInfo = await TokenInfo(cancellationToken);
        if (tokenInfo is not { Active: true, Revoked: false })
        {
            throw new GitLabException(System.Net.HttpStatusCode.Unauthorized, "Token is inactive or revoked.");
        }

        var tokenLifeSpan = tokenInfo.ExpiresAt.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow;
        CheckTokenExpirationWarning(tokenLifeSpan);

        var applications = new ConcurrentBag<Application>();
        var projects = await gitLabClient.Projects.GetAsync(opt => { });
        using var semaphore = new SemaphoreSlim(maximumConcurrencyCalls, maximumConcurrencyCalls);
        try
        {
            await Parallel.ForEachAsync(projects.Where<Project>(p => !p.Archived), cancellationToken, async (project, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", RepositoryType.Gitlab, project.Name);
                    var application = await ScanProjectAsync(project, cancellationToken);
                    if (application is not null)
                    {
                        applications.Add(application);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogDebug("Scan failed for {ScannerType} Repository '{RepositoryName}' : {ExceptionMessage}", RepositoryType.Gitlab, project.Name, ex.Message);
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
            logger.LogWarning("Operation cancelled.");
            return [];
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled.");
            return [];
        }

        return applications;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        HttpClient.Dispose();
    }

    private static string BranchLinkSuffix(string branchName)
    => $"-/tree/{branchName}";

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
            logger.LogWarning("Application {ApplicationName} skipped because of Gitlab Error : {ExceptionMessage}.", project.Name, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", project.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private void CheckTokenExpirationWarning(TimeSpan tokenLifeSpan)
    {
        if (tokenLifeSpan < tokenExpirationWarningThreshold)
        {
            logger.LogWarning("Token is expiring soon : {TokenLifeSpan}", tokenLifeSpan.ToString(@"dd\:hh\:mm\:ss"));
        }
    }

    private Task<PersonnalAccessToken?> TokenInfo(CancellationToken cancellationToken)
        => HttpClient.GetFromJsonAsync<PersonnalAccessToken>("api/v4/personal_access_tokens/self", cancellationToken);

    private async Task<IReadOnlyCollection<File>> FindModuleFiles(int projectId, string branchName)
    {
        var tree = await gitLabClient.Trees.GetAsync(projectId, o => { o.Recursive = true; o.Reference = branchName; });
        var fileHeaders = tree.Where(f => moduleParsers.Any(p => p.IsModuleFile(f.Path)));
        var filesTask = fileHeaders.Select(fh => gitLabClient.Files.GetAsync(projectId, fh.Path, branchName));
        return await Task.WhenAll(filesTask);
    }

    private async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(int projectId)
    {
        var branches = await gitLabClient.Branches.GetAsync(projectId, o => { });
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
                logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Filename, branchName, project.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<ApplicationModule?> ScanModuleAsync(File moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = moduleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.ContentDecoded));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.ContentDecoded, moduleFile.Filename, cancellationToken);
    }
}
