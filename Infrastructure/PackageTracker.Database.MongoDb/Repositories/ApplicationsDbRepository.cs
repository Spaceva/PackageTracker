using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.Database.Common.Enrichers;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Database.MongoDb.Repositories.Enrichers;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package;
using System.Web;

namespace PackageTracker.Database.MongoDb.Repositories;
internal class ApplicationsDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IPackagesRepository? packagesRepository, [FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IFrameworkRepository? frameworksRepository, MongoDbContext dbContext, ILogger<ApplicationsDbRepository> logger) : BaseDbRepository<ApplicationDbModel>(dbContext, logger), IApplicationsRepository
{
    private bool HasCache => packagesRepository is not null && frameworksRepository is not null;

    public async Task<bool> ExistsAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        return await AnyAsync(Filter.Eq(a => a.Name, name) & Filter.Eq(a => a.AppType, applicationType.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(repositoryLink)), cancellationToken);
    }

    public async Task DeleteAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        await DeleteByQueryAsync(Filter.Eq(a => a.Name, name) & Filter.Eq(a => a.AppType, applicationType.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(repositoryLink)), cancellationToken);
    }

    public async Task<Application> GetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        return await TryGetAsync(name, applicationType, repositoryLink, cancellationToken) ?? throw new ApplicationNotFoundException();
    }

    public async Task SaveAsync(Application application, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(Filter.Eq(a => a.Name, application.Name) & Filter.Eq(a => a.AppType, application.Type.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(application.RepositoryLink)), new ApplicationDbModel(application), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Application>> SearchAsync(ApplicationSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        var results = await FindAsync(searchCriteria.ToFilterDefinition(), cancellationToken);

        IReadOnlyCollection<Application> applications = [.. results.Select(app => app.ToDomain())];

        IApplicationEnricher enricher = HasCache ? new ApplicationWithCacheEnricher(packagesRepository!, frameworksRepository!, searchCriteria.ShowOnlyTracked) : new Repositories.Enrichers.ApplicationNoCacheEnricher(DbContext, searchCriteria.ShowOnlyTracked);
        await enricher.EnrichApplicationsAsync(applications, cancellationToken);

        return applications;
    }

    public async Task<Application?> TryGetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        var applicationDb = await GetAsync(Filter.Eq(a => a.Name, name) & Filter.Eq(a => a.AppType, applicationType.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(repositoryLink)), cancellationToken);
        if (applicationDb is null)
        {
            return null;
        }

        var application = applicationDb.ToDomain();

        IApplicationEnricher enricher = HasCache ? new ApplicationWithCacheEnricher(packagesRepository!, frameworksRepository!) : new Repositories.Enrichers.ApplicationNoCacheEnricher(DbContext);
        await enricher.EnrichApplicationAsync(application, cancellationToken);

        return application;
    }
}
