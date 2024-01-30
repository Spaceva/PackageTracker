namespace PackageTracker.Domain.Application;

using PackageTracker.Domain.Application.Model;

public interface IApplicationsRepository
{
    Task<Application> GetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default);

    Task<Application?> TryGetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default);

    Task DeleteAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Application>> SearchAsync(ApplicationSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default);

    Task SaveAsync(Application application, CancellationToken cancellationToken = default);
}
