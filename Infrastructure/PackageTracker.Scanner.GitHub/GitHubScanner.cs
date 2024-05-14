using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;
using Application = PackageTracker.Domain.Application.Model.Application;
using RepositoryType = PackageTracker.Domain.Application.Model.RepositoryType;

namespace PackageTracker.Scanner.GitHub;

internal class GitHubScanner(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IMediator mediator)
    : BaseScanner<Repository>(trackedApplication, moduleParsers, logger, mediator)
{
    private const string GITHUB_MAIN_HOST = "https://github.com/";

    private readonly IGitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue($"PackageTracker-Scanner-{trackedApplication.ScannerName}")) { Credentials = new Credentials(trackedApplication.AccessToken), };

    protected override RepositoryType RepositoryType => RepositoryType.GitHub;

    protected override UntypedApplication AsUntypedApplication(Repository repository)
     => new() { Name = repository.Name, Path = repository.FullName.Replace("/", ">"), RepositoryLink = repository.HtmlUrl, Branchs = [], RepositoryType = RepositoryType.GitHub };

    protected override string BranchLinkSuffix(string branchName) => $"/tree/{branchName}";

    protected override async Task CheckTokenExpirationAsync(CancellationToken cancellationToken)
    {
        var rateLimits = await gitHubClient.RateLimit.GetRateLimits();
        if (rateLimits.Rate.Remaining == 0)
        {
            throw new ApiException("Rate limit exceeded", System.Net.HttpStatusCode.TooManyRequests);
        }

        if ((double)rateLimits.Rate.Remaining / rateLimits.Rate.Limit < 0.15d)
        {
            Logger.LogWarning("Rate limit almost reached: {Remaining} remaining.", rateLimits.Rate.Remaining);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        // Left empty intentionally.
    }

    protected override async Task<IEnumerable<Repository>> GetRepositoriesAsync(CancellationToken cancellationToken) => await getRepositoriesDelegate(gitHubClient, OrganizationOrUserName);

    protected override bool IsNotArchived(Repository repository) => !repository.Archived;

    protected override string NameOf(Repository repository) => repository.Name;

    protected override async Task<Application?> ScanRepositoryAsync(Repository repository, CancellationToken cancellationToken)
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
            Logger.LogWarning("Application {ApplicationName} skipped because of GitHub Error : {ExceptionMessage}.", repository.Name, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private async Task<IReadOnlyCollection<RepositoryContent>> FindModuleFiles(Repository repository, Branch branch)
    {
        var treeResponse = await gitHubClient.Git.Tree.GetRecursive(repository.Id, branch.Commit.Sha);
        var fileHeaders = treeResponse.Tree.Where(t => ModuleParsers.Any(mp => mp.IsModuleFile(t.Path)));
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
                Logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Name, branch.Name, repository.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<ApplicationModule?> ScanModuleAsync(RepositoryContent moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = ModuleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.Content));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.Content, moduleFile.Name, cancellationToken);
    }

    private string OrganizationOrUserName => TrackedApplication.RepositoryRootLink.Replace(GITHUB_MAIN_HOST, string.Empty);
}
