using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.Common.Enrichers;
using PackageTracker.Database.EntityFramework.Extensions;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationsDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IPackagesRepository? packagesRepository, [FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IFrameworkRepository? frameworksRepository, IServiceScopeFactory serviceScopeFactory) : IApplicationsRepository
{
    private bool HasCache => packagesRepository is not null && frameworksRepository is not null;

    public async Task<bool> ExistsAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await dbContext.Applications.AnyAsync(a => a.Name.Equals($"{name} ({applicationType})") && a.RepositoryLink.Equals(repositoryLink), cancellationToken);
    }

    public async Task SaveAsync(Application application, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var applicationFromDb = await TryGetAsync(application.Name, application.Type, application.RepositoryLink, cancellationToken);
        if (applicationFromDb is not null)
        {
            applicationFromDb.Name = $"{applicationFromDb.Name} ({applicationFromDb.Type})";
            applicationFromDb.Branchs = application.Branchs;
            applicationFromDb.IsSoonDecommissioned = application.IsSoonDecommissioned;
            applicationFromDb.IsDeadLink = application.IsDeadLink;
            dbContext.Entry(applicationFromDb).State = EntityState.Modified;
            dbContext.SaveChanges();
            return;
        }

        var oldName = application.Name;
        application.Name = $"{application.Name} ({application.Type})";
        dbContext.Applications.Add(application);
        dbContext.SaveChanges();
        application.Name = oldName;
    }

    public async Task<Application> GetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
        => (await TryGetAsync(name, applicationType, repositoryLink, cancellationToken)) ?? throw new ApplicationNotFoundException();

    public async Task<Application?> TryGetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var existingApplication = await dbContext.Applications.FindAsync([$"{name} ({applicationType})", repositoryLink], cancellationToken);
        if (existingApplication is null)
        {
            return null;
        }

        IApplicationEnricher enricher = HasCache ? new ApplicationWithCacheEnricher(packagesRepository!, frameworksRepository!) : new Repositories.Enrichers.ApplicationNoCacheEnricher(dbContext);
        await enricher.EnrichApplicationAsync(existingApplication, cancellationToken);

        return existingApplication;
    }

    public async Task DeleteAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var existingApplication = await dbContext.Applications.FindAsync([$"{name} ({applicationType})", repositoryLink], cancellationToken) ?? throw new ApplicationNotFoundException();

        dbContext.Applications.Remove(existingApplication);
        dbContext.SaveChanges();
    }

    public async Task<IReadOnlyCollection<Application>> SearchAsync(ApplicationSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var applications = await dbContext.Applications.ApplySearchCriteria(searchCriteria).ToArrayAsync(cancellationToken);

        var query = applications.AsQueryable()
                                .FilterByApplicationTypes(searchCriteria.ApplicationTypes)
                                .FilterByLastCommitAfter(searchCriteria)
                                .FilterByLastCommitBefore(searchCriteria);

        applications = [.. query];

        IApplicationEnricher enricher = HasCache ? new ApplicationWithCacheEnricher(packagesRepository!, frameworksRepository!, searchCriteria.ShowOnlyTracked) : new Repositories.Enrichers.ApplicationNoCacheEnricher(dbContext, searchCriteria.ShowOnlyTracked);
        await enricher.EnrichApplicationsAsync(applications, cancellationToken);
        
        query = applications.AsQueryable().FilterByFrameworkStatus(searchCriteria).ApplyPagination(a => a.Name, skip, take);
        return [.. query];
    }
}
