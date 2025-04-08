using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using System.Collections.Concurrent;
using static PackageTracker.Scanner.ScannerSettings;
using PackageTracker.SharedKernel.Mediator;

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
        var response = await mediator.Query<GetApplicationsQuery, GetApplicationsQueryResponse>(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { RepositoryTypes = [RepositoryType], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await GetRepositoriesAsync(cancellationToken);
        var remoteApplications = repositories.Where(IsNotArchived).Select(AsUntypedApplication);

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    public async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        await CheckTokenExpirationAsync(cancellationToken);

        var applications = new ConcurrentBag<Application>();
        var repositories = await GetRepositoriesAsync(cancellationToken);
        using var semaphore = new SemaphoreSlim(MaximumConcurrencyCalls, MaximumConcurrencyCalls);

        try
        {
            await Parallel.ForEachAsync(repositories.Where(IsNotArchived), cancellationToken, async (repository, cancellationToken) =>
            {
                try
                {
                    await semaphore.WaitAsync(cancellationToken);
                    Logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", RepositoryType, NameOf(repository));
                    var application = await ScanRepositoryAsync(repository, cancellationToken);
                    if (application is not null)
                    {
                        applications.Add(application);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Scan failed for {ScannerType} Repository '{RepositoryName}'.", RepositoryType, NameOf(repository));
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

    protected IEnumerable<IApplicationModuleParser> ModuleParsers => moduleParsers;

    protected ILogger Logger => logger;

    protected TrackedApplication TrackedApplication => trackedApplication;

    protected int MaximumConcurrencyCalls => TrackedApplication.MaximumConcurrencyCalls;

    protected abstract RepositoryType RepositoryType { get; }

    protected abstract UntypedApplication AsUntypedApplication(TRepository repository);

    protected abstract string BranchLinkSuffix(string branchName);

    protected abstract Task CheckTokenExpirationAsync(CancellationToken cancellationToken);

    protected abstract void Dispose(bool isDisposing);

    protected abstract Task<IEnumerable<TRepository>> GetRepositoriesAsync(CancellationToken cancellationToken);

    protected abstract bool IsNotArchived(TRepository repository);

    protected abstract string NameOf(TRepository repository);

    protected abstract Task<Application?> ScanRepositoryAsync(TRepository repository, CancellationToken cancellationToken);
}
