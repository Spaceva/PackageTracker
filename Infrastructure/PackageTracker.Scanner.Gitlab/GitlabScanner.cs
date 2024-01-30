using GitLabApiClient;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using static PackageTracker.Scanner.ScannerSettings;

namespace PackageTracker.Scanner.Gitlab;

internal abstract class GitlabScanner : IApplicationsScanner
{
    private static readonly TimeSpan DefaultTokenExpirationWarningThreshold = TimeSpan.FromDays(7);

    protected GitlabScanner(TrackedApplication trackedApplication, IMediator mediator, ILogger logger)
    {
        GitLabClient = new(trackedApplication.RepositoryRootLink, trackedApplication.AccessToken);

        HttpClient = new HttpClient
        {
            BaseAddress = new Uri(trackedApplication.RepositoryRootLink),

        };
        HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + trackedApplication.AccessToken);

        MaximumConcurrencyCalls = trackedApplication.MaximumConcurrencyCalls;

        TokenExpirationWarningThreshold = trackedApplication.TokenExpirationWarningThreshold ?? DefaultTokenExpirationWarningThreshold;

        Logger = logger;

        Mediator = mediator;
    }

    private protected IMediator Mediator { get; }

    private protected ILogger Logger { get; }

    private protected GitLabClient GitLabClient { get; }

    private protected HttpClient HttpClient { get; }

    private protected TimeSpan TokenExpirationWarningThreshold { get; }

    private protected int MaximumConcurrencyCalls { get; }

    public abstract Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken);

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { ApplicationTypes = [LookedUpApplicationType], RepositoryTypes = [RepositoryType.Gitlab], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var projects = await GitLabClient.Projects.GetAsync(opt => { });
        var remoteApplications = projects.Where(p => !p.Archived).Select(project => Application(project.Name, project.Namespace.FullPath.Replace("/", ">"), project.HttpUrlToRepo.Replace(".git", "/"), [])).ToArray();

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
        HttpClient.Dispose();
    }
}
