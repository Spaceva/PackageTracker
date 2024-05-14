using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using System.Collections.Concurrent;
using static PackageTracker.Scanner.ScannerSettings;
using Application = PackageTracker.Domain.Application.Model.Application;
using RepositoryType = PackageTracker.Domain.Application.Model.RepositoryType;

namespace PackageTracker.Scanner.GitHub;

internal class GitHubScanner(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger) : IApplicationsScanner
{
    private const string GITHUB_MAIN_HOST = "https://github.com/";

    private readonly IGitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue($"PackageTracker-Scanner-{trackedApplication.ScannerName}"))
    {
        Credentials = new Credentials(trackedApplication.AccessToken),
    };

    private readonly int maximumConcurrencyCalls = trackedApplication.MaximumConcurrencyCalls;

    private readonly string organizationOrUserName = trackedApplication.RepositoryRootLink.Replace(GITHUB_MAIN_HOST, string.Empty);

    private readonly Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositories = getRepositoriesDelegate;

    public async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var rateLimits = await gitHubClient.RateLimit.GetRateLimits();
        if (rateLimits.Rate.Remaining == 0)
        {
            throw new ApiException("Rate limit exceeded", System.Net.HttpStatusCode.TooManyRequests);
        }
        else if ((double)rateLimits.Rate.Remaining / rateLimits.Rate.Limit < 0.15d)
        {
            logger.LogWarning("Rate limit almost reached: {Remaining} remaining.", rateLimits.Rate.Remaining);
        }

        var applications = new ConcurrentBag<Application>();
        var repositories = await getRepositories(gitHubClient, organizationOrUserName);
        using var semaphore = new SemaphoreSlim(maximumConcurrencyCalls, maximumConcurrencyCalls);
        try
        {
            await Parallel.ForEachAsync(repositories, cancellationToken, async (repository, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", Domain.Application.Model.RepositoryType.GitHub, repository.Name);
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

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { RepositoryTypes = [RepositoryType.GitHub], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await getRepositories(gitHubClient, organizationOrUserName);
        var remoteApplications = repositories.Where(p => !p.Archived).Select(repo => new UntypedApplication { Name = repo.Name, Path = repo.FullName.Replace("/", ">"), RepositoryLink = repo.HtmlUrl, Branchs = [] }).ToArray();

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Left empty intentionally
    }

    private static string BranchLinkSuffix(string branchName)
    => $"/tree/{branchName}";

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
                var commitInfo = await gitHubClient.Git.Commit.Get(repository.Id, commitSha);
                var lastCommitDate = commitInfo.Committer.Date;

                applicationBranchs.Add(ApplicationBranch.From(branch.Name, repository.HtmlUrl + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application.From(repository.Name, repository.FullName.Replace("/", ">").Replace($">{repository.Name}", string.Empty), repository.HtmlUrl, applicationBranchs, RepositoryType.Gitlab);
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
            logger.LogWarning("Application {ApplicationName} skipped because of GitHub Error : {ExceptionMessage}.", repository.Name, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private async Task<IReadOnlyCollection<RepositoryContent>> FindModuleFiles(Repository repository, Branch branch)
    {
        var treeResponse = await gitHubClient.Git.Tree.GetRecursive(repository.Id, branch.Commit.Sha);
        var fileHeaders = treeResponse.Tree.Where(t => moduleParsers.Any(mp => mp.IsModuleFile(t.Path)));
        var filesTask = fileHeaders.Select(fh => gitHubClient.Repository.Content.GetAllContentsByRef(repository.Owner.Login, repository.Name, fh.Path, branch.Commit.Sha));
        var content = await Task.WhenAll(filesTask);
        return [.. content.Select(a => a[0])];
    }

    private async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(long repositoryId)
    {
        var branches = await gitHubClient.Repository.Branch.GetAll(repositoryId);
        var branchsNames = branches.Select(b => b.Name).Intersect(Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<ApplicationModule>> ScanBranchAsync(Repository repository, Branch branch, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(repository, branch);
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
                logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Name, branch.Name, repository.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<ApplicationModule?> ScanModuleAsync(RepositoryContent moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = moduleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.Content));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.Content, moduleFile.Name, cancellationToken);
    }
}
