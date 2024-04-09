using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MongoDb.Repositories;

internal class PackagesDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IPackagesRepository? cacheRepository, MongoDbContext dbContext, ILogger<PackagesDbRepository> logger) : BaseDbRepository<PackageDbModel>(dbContext, logger), IPackagesRepository
{
    public async Task<bool> ExistsAsync(string packageName, CancellationToken cancellationToken = default)
    {
        if (cacheRepository is null)
        {
            return await ExistsNoCacheAsync(packageName, cancellationToken);
        }

        return await cacheRepository.ExistsAsync(packageName, cancellationToken) || await ExistsNoCacheAsync(packageName, cancellationToken);
    }

    public async Task AddAsync(Package package, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(Filter.Eq(p => p.Name, package.Name), new PackageDbModel(package), cancellationToken);
        if (cacheRepository is not null)
        {
            await cacheRepository.AddAsync(package, cancellationToken);
        }
    }

    public async Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        await DeleteByQueryAsync(Filter.Eq(p => p.Name, packageName), cancellationToken);
        if (cacheRepository is not null)
        {
            await cacheRepository.DeleteByNameAsync(packageName, cancellationToken);
        }
    }

    public async Task<IReadOnlyCollection<Package>> GetAllAsync(string? name = null, IReadOnlyCollection<PackageType>? packageTypes = null, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Filter.Empty;
        if (!string.IsNullOrWhiteSpace(name))
        {
            filterDefinition &= Filter.Eq(p => p.Name, name);
        }

        if (packageTypes is not null && packageTypes.Count > 0)
        {
            filterDefinition &= Filter.In(nameof(PackageDbModel.Type).ToCamelCase(), packageTypes);
        }

        var packagesDb = await FindAsync(filterDefinition, cancellationToken);
        return [.. packagesDb.Select(p => p.ToDomain())];
    }

    public async Task<Package> GetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        return await TryGetByNameAsync(packageName, cancellationToken) ?? throw new PackageNotFoundException();
    }

    public async Task<PackageVersion> GetVersionAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default)
    {
        if (cacheRepository is null)
        {
            return await GetVersionNoCacheAsync(packageName, versionLabel, cancellationToken);
        }

        try
        {
            return await cacheRepository.GetVersionAsync(packageName, versionLabel, cancellationToken);
        }
        catch (Exception)
        {
            return await GetVersionNoCacheAsync(packageName, versionLabel, cancellationToken);
        }
    }

    public async Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        if (cacheRepository is null)
        {
            return await TryGetByNameNoCacheAsync(packageName, cancellationToken);
        }

        var package = await cacheRepository.TryGetByNameAsync(packageName, cancellationToken);
        if (package is not null)
        {
            return package;
        }

        package = await TryGetByNameNoCacheAsync(packageName, cancellationToken);
        if (package is not null)
        {
            await cacheRepository.AddAsync(package, cancellationToken);
        }
        return package;
    }

    public async Task UpdateAsync(Package package, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(Filter.Eq(p => p.Name, package.Name), new PackageDbModel(package), cancellationToken);
        if (cacheRepository is not null)
        {
            await cacheRepository.UpdateAsync(package, cancellationToken);
        }
    }

    private async Task<PackageVersion> GetVersionNoCacheAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default)
    {
        var package = await GetByNameAsync(packageName, cancellationToken);
        return package.Versions.SingleOrDefault(v => v.ToString().Equals(versionLabel)) ?? throw new PackageVersionNotFoundException();
    }

    private async Task<Package?> TryGetByNameNoCacheAsync(string packageName, CancellationToken cancellationToken = default)
    {
        var packagesDb = await FindAsync(Builders<PackageDbModel>.Filter.Eq(p => p.Name, packageName), cancellationToken);
        var packageDb = packagesDb.SingleOrDefault();
        return packageDb?.ToDomain();
    }

    private async Task<bool> ExistsNoCacheAsync(string packageName, CancellationToken cancellationToken = default)
    {
        return await AnyAsync(Builders<PackageDbModel>.Filter.Eq(p => p.Name, packageName), cancellationToken);
    }
}
