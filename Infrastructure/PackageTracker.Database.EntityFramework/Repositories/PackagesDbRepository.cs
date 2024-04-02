using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework;
internal class PackagesDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IPackagesRepository? cacheRepository, IServiceScopeFactory serviceScopeFactory) : IPackagesRepository
{
    public async Task AddAsync(Package package, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var packageFromDb = await TryGetByNameAsync(package.Name, dbContext, cancellationToken);
        if (packageFromDb is not null)
        {
            throw new PackageAlreadyExistsException();
        }

        dbContext.Packages.Add(package);
        dbContext.SaveChanges();
    }

    public async Task<IReadOnlyCollection<Package>> GetAllAsync(string? name = null, IReadOnlyCollection<PackageType>? packageTypes = null, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var query = dbContext.Packages.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }
        var packages = await query.ToArrayAsync(cancellationToken);

        if (packageTypes is not null)
        {
            return [.. packages.Where(p => packageTypes.Contains(p.Type))];
        }

        return packages;
    }

    public async Task<Package> GetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await GetByNameAsync(packageName, dbContext, cancellationToken);
    }

    public async Task<PackageVersion> GetVersionAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var package = await GetByNameAsync(packageName, dbContext, cancellationToken);
        return package.Versions.FirstOrDefault(v => v.Equals(versionLabel)) ?? throw new PackageVersionNotFoundException();
    }

    public async Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await TryGetByNameAsync(packageName, dbContext, cancellationToken);
    }

    public async Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var existingPackage = await GetByNameAsync(packageName, dbContext, cancellationToken);
        dbContext.Packages.Remove(existingPackage);
        dbContext.SaveChanges();
    }

    public async Task UpdateAsync(Package package, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var packageFromDb = await GetByNameAsync(package.Name, dbContext, cancellationToken);
        packageFromDb.Link = package.Link;
        packageFromDb.RegistryUrl = package.RegistryUrl;
        packageFromDb.Versions = package.Versions;
        dbContext.SaveChanges();
    }

    private static async Task<Package?> TryGetByNameAsync(string packageName, PackageTrackerDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return await dbContext.Packages.FindAsync([packageName], cancellationToken);
    }

    private static async Task<Package> GetByNameAsync(string packageName, PackageTrackerDbContext dbContext, CancellationToken cancellationToken = default)
    {
        return await TryGetByNameAsync(packageName, dbContext, cancellationToken) ?? throw new PackageNotFoundException();
    }
}
