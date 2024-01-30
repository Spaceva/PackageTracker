using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.EntityFramework.Extensions;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationsDbRepository(IServiceScopeFactory serviceScopeFactory) : IApplicationsRepository
{
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

        await EnrichApplicationAsync(existingApplication, dbContext, false, cancellationToken);
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

        await EnrichApplicationsAsync(applications, dbContext, searchCriteria.ShowOnlyTracked, cancellationToken);

        query = applications.AsQueryable().FilterByFrameworkStatus(searchCriteria).ApplyPagination(a => a.Name, skip, take);
        return [.. query];
    }
    private static Task EnrichApplicationAsync(Application application, PackageTrackerDbContext dbContext, bool showOnlyTrackedPackages = false, CancellationToken cancellationToken = default)
        => EnrichApplicationsAsync(new[] { application }, dbContext, showOnlyTrackedPackages, cancellationToken);

    private static async Task EnrichApplicationsAsync(IReadOnlyCollection<Application> applications, PackageTrackerDbContext dbContext, bool showOnlyTrackedPackages = false, CancellationToken cancellationToken = default)
    {
        var allPackagesName = applications.SelectMany(a => a.Branchs.SelectMany(b => b.Modules.SelectMany(b => b.Packages.Select(b => b.PackageName)))).Distinct().ToArray();
        var packages = await GetAllPackagesAsync(allPackagesName, dbContext, cancellationToken);
        var frameworks = await GetAllFrameworksAsync(dbContext, cancellationToken);
        foreach (var application in applications)
        {
            EnrichApplication(application, packages, frameworks, showOnlyTrackedPackages, cancellationToken);
        }
    }

    private static async Task<IDictionary<string, Package>> GetAllPackagesAsync(IReadOnlyCollection<string> packagesName, PackageTrackerDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return await dbContext.Packages.AsNoTracking().Where(p => packagesName.Contains(p.Name)).ToDictionaryAsync(x => x.Name, cancellationToken: cancellationToken);
    }

    private static async Task<IDictionary<(string Name, string Version), Framework>> GetAllFrameworksAsync(PackageTrackerDbContext dbContext, CancellationToken cancellationToken)
    {
        return await dbContext.Frameworks.AsNoTracking().ToDictionaryAsync(f => (f.Name, f.Version), cancellationToken);
    }

    private static void EnrichApplication(Application application, IDictionary<string, Package> packagesByName, IDictionary<(string Name, string Version), Framework> frameworks, bool showOnlyTrackedPackages = false, CancellationToken cancellationToken = default)
    {
        application.Name = application.Name.Replace($" ({application.Type})", string.Empty);
        foreach (var branch in application.Branchs)
        {
            foreach (var module in branch.Modules)
            {
                module.Framework = MatchFramework(module, frameworks);
                foreach (var package in module.Packages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!packagesByName.TryGetValue(package.PackageName, out var packageFromDb))
                    {
                        continue;
                    }

                    package.TrackedPackage = packageFromDb;
                    package.TrackedPackageVersion = packageFromDb.Versions.FirstOrDefault(p => p.Equals(package.PackageVersion));
                }

                if (showOnlyTrackedPackages)
                {
                    module.Packages = [.. module.Packages.Where(p => p.IsPackageTracked)];
                }
            }
        }
    }

    private static Framework? MatchFramework(ApplicationModule module, IDictionary<(string Name, string Version), Framework> frameworks)
    {
        if (module is DotNetAssembly dotNetAssembly)
        {
            _ = frameworks.TryGetValue((DotNetAssembly.FrameworkName, dotNetAssembly.DotNetVersion + ".0"), out var framework)
                || frameworks.TryGetValue((DotNetAssembly.FrameworkNameLegacy, dotNetAssembly.DotNetVersion.Replace("Framework", string.Empty).Trim()), out framework)
                || frameworks.TryGetValue((DotNetAssembly.FrameworkNameStandard, dotNetAssembly.DotNetVersion.Replace("Standard", string.Empty).Trim()), out framework);
            return framework;
        }

        if (module is AngularModule angularModule)
        {
            frameworks.TryGetValue((AngularModule.FrameworkName, angularModule.AngularVersion), out var framework);
            return framework;
        }

        if (module is PhpModule phpModule)
        {
            _ = frameworks.TryGetValue((PhpModule.FrameworkName, phpModule.PhpVersion), out var framework)
                || frameworks.TryGetValue((PhpModule.FrameworkName, new PackageVersion(phpModule.PhpVersion).ToStringMajorMinor()), out framework);
            return framework;
        }

        return null;
    }
}
