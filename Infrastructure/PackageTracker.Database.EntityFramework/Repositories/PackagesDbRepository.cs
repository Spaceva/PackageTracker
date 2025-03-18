using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;
internal class PackagesDbRepository(IServiceScopeFactory serviceScopeFactory) : IPackagesRepository
{
    public async Task<bool> ExistsAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cacheRepository = scope.ServiceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is null)
        {
            return await ExistsNoCacheAsync(packageName, cancellationToken);
        }

        return await cacheRepository.ExistsAsync(packageName, cancellationToken) || await ExistsNoCacheAsync(packageName, cancellationToken);
    }

    public async Task AddAsync(Package package, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var packageFromDb = await dbContext.Packages.FindAsync([package.Name], cancellationToken);
        if (packageFromDb is not null)
        {
            throw new PackageAlreadyExistsException();
        }

        dbContext.Packages.Add(package);
        dbContext.SaveChanges();

        var cacheRepository = scope.ServiceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is not null)
        {
            await cacheRepository.AddAsync(package, cancellationToken);
        }
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
        return await TryGetByNameAsync(packageName, cancellationToken) ?? throw new PackageNotFoundException();
    }

    public async Task<PackageVersion> GetVersionAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default)
    {
        var package = await GetByNameAsync(packageName, cancellationToken);
        return package.Versions.FirstOrDefault(v => v.Equals(versionLabel)) ?? throw new PackageVersionNotFoundException();
    }

    public async Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cacheRepository = scope.ServiceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is null)
        {
            return await TryGetByNameNoCacheAsync(packageName, cancellationToken);
        }

        var cachedPackaged = await cacheRepository.TryGetByNameAsync(packageName, cancellationToken);
        if (cachedPackaged is not null)
        {
            return cachedPackaged;
        }

        var package = await TryGetByNameNoCacheAsync(packageName, cancellationToken);
        if (package is not null)
        {
            await cacheRepository.AddAsync(package, cancellationToken);
        }

        return package;
    }

    public async Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var existingPackage = await dbContext.Packages.FindAsync([packageName], cancellationToken) ?? throw new PackageNotFoundException();
        dbContext.Packages.Remove(existingPackage);
        dbContext.SaveChanges();

        var cacheRepository = scope.ServiceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is not null)
        {
            await cacheRepository.DeleteByNameAsync(packageName, cancellationToken);
        }
    }

    public async Task UpdateAsync(Package package, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var packageFromDb = await dbContext.Packages.FindAsync([package.Name], cancellationToken) ?? throw new PackageNotFoundException();
        packageFromDb.Link = package.Link;
        packageFromDb.RegistryUrl = package.RegistryUrl;
        packageFromDb.Versions = package.Versions;
        dbContext.SaveChanges();

        var cacheRepository = scope.ServiceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is not null)
        {
            await cacheRepository.UpdateAsync(packageFromDb, cancellationToken);
        }
    }

    private async Task<bool> ExistsNoCacheAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await dbContext.Packages.AnyAsync(p => p.Name.Equals(packageName), cancellationToken);
    }

    private async Task<Package?> TryGetByNameNoCacheAsync(string packageName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await dbContext.Packages.FindAsync([packageName], cancellationToken);
    }
}
