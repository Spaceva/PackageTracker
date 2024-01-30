namespace PackageTracker.Domain.Application;

public interface IApplicationsScanner : IDisposable
{
    Task<IReadOnlyCollection<Model.Application>> ScanRemoteAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Model.Application>> FindDeadLinksAsync(CancellationToken cancellationToken);
}
