using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using static PackageTracker.Scanner.ScannerSettings;

namespace PackageTracker.Scanner;
public abstract class BaseScanner<TRepository>(TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IMediator mediator) : IApplicationsScanner
{
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { RepositoryTypes = [RepositoryType], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await GetRepositoriesAsync(cancellationToken);
        var remoteApplications = repositories.Where(IsNotArchived).Select(AsUntypedApplication);

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    public abstract Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken);

    protected IEnumerable<IApplicationModuleParser> ModuleParsers => moduleParsers;

    protected ILogger Logger => logger;

    protected TrackedApplication TrackedApplication => trackedApplication;

    protected int MaximumConcurrencyCalls => TrackedApplication.MaximumConcurrencyCalls;

    protected abstract RepositoryType RepositoryType { get; }

    protected abstract Task<IEnumerable<TRepository>> GetRepositoriesAsync(CancellationToken cancellationToken);

    protected abstract bool IsNotArchived(TRepository repository);

    protected abstract UntypedApplication AsUntypedApplication(TRepository repository);

    protected abstract string BranchLinkSuffix(string branchName);

    protected abstract void Dispose(bool isDisposing);
}
