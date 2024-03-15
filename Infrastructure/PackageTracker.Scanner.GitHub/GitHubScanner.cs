using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using static PackageTracker.Scanner.ScannerSettings;
using Application = PackageTracker.Domain.Application.Model.Application;
using RepositoryType = PackageTracker.Domain.Application.Model.RepositoryType;

namespace PackageTracker.Scanner.GitHub;

internal abstract class GitHubScanner : IApplicationsScanner
{
    private const string GITHUB_MAIN_HOST = "https://github.com/";

    protected GitHubScanner(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, TrackedApplication trackedApplication, IMediator mediator, ILogger logger)
    {
        GitHubClient = new GitHubClient(new ProductHeaderValue($"PackageTracker-Scanner-{trackedApplication.ScannerName}"))
        {
            Credentials = new Credentials(trackedApplication.AccessToken)
        };

        MaximumConcurrencyCalls = trackedApplication.MaximumConcurrencyCalls;

        Logger = logger;

        Mediator = mediator;

        OrganizationOrUserName = trackedApplication.RepositoryRootLink.Replace(GITHUB_MAIN_HOST, string.Empty);

        GetRepositories = getRepositoriesDelegate;
    }

    private protected IMediator Mediator { get; }

    private protected ILogger Logger { get; }

    private protected IGitHubClient GitHubClient { get; }

    private protected int MaximumConcurrencyCalls { get; }

    private protected string OrganizationOrUserName { get; }

    private protected Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> GetRepositories { get; }

    public abstract Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken);

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { ApplicationTypes = [LookedUpApplicationType], RepositoryTypes = [RepositoryType.GitHub], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await GetRepositories(GitHubClient, OrganizationOrUserName);
        var remoteApplications = repositories.Where(p => !p.Archived).Select(repo => Application(repo.Name, repo.FullName.Replace("/", ">"), repo.HtmlUrl, [])).ToArray();

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    private protected abstract Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches);

    private protected abstract ApplicationType LookedUpApplicationType { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Left empty intentionally
    }
}
