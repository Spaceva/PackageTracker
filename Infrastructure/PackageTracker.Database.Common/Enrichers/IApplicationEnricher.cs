using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.Common.Enrichers;
public interface IApplicationEnricher
{
    Task EnrichApplicationAsync(Application application, CancellationToken cancellationToken = default);

    Task EnrichApplicationsAsync(IEnumerable<Application> applications, CancellationToken cancellationToken);
}
