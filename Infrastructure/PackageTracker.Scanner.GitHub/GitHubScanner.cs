using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using static PackageTracker.Scanner.ScannerSettings;
using Application = PackageTracker.Domain.Application.Model.Application;

namespace PackageTracker.Scanner.GitHub;

internal abstract class GitHubScanner : IApplicationsScanner
{
    private static readonly TimeSpan DefaultTokenExpirationWarningThreshold = TimeSpan.FromDays(7);

    protected GitHubScanner(TrackedApplication trackedApplication, IMediator mediator, ILogger logger)
    {
        GitHubClient = new GitHubClient(new ProductHeaderValue($"PackageTracker - Scanner {trackedApplication.ScannerName}"))
        {
            Credentials = new Credentials(trackedApplication.AccessToken)
        };

        MaximumConcurrencyCalls = trackedApplication.MaximumConcurrencyCalls;

        TokenExpirationWarningThreshold = trackedApplication.TokenExpirationWarningThreshold ?? DefaultTokenExpirationWarningThreshold;

        Logger = logger;

        Mediator = mediator;

        OrganizationName = trackedApplication.RepositoryRootLink;
    }

    private protected IMediator Mediator { get; }

    private protected ILogger Logger { get; }

    private protected IGitHubClient GitHubClient { get; }

    private protected TimeSpan TokenExpirationWarningThreshold { get; }

    private protected int MaximumConcurrencyCalls { get; }

    private protected string OrganizationName { get; }

    public abstract Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken);

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { ApplicationTypes = [LookedUpApplicationType], RepositoryTypes = [RepositoryType.Gitlab], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await GitHubClient.Repository.GetAllForOrg(OrganizationName);
        var remoteApplications = repositories.Where(p => !p.Archived).Select(project => Application(project.Name, project.FullName.Replace("/", ">"), project.Url, [])).ToArray();

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
