using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MongoDb.Repositories;

internal class PackagesDbRepository(MongoDbContext dbContext, ILogger<PackagesDbRepository> logger) : BaseDbRepository<PackageDbModel>(dbContext, logger), IPackagesRepository
{
    public async Task AddAsync(Package package, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(Filter.Eq(p => p.Name, package.Name), new PackageDbModel(package), cancellationToken);
    }

    public async Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        await DeleteByQueryAsync(Filter.Eq(p => p.Name, packageName), cancellationToken);
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
            filterDefinition &= Filter.AnyIn(nameof(PackageDbModel.Type).ToCamelCase(), packageTypes);
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
        var package = await GetByNameAsync(packageName, cancellationToken);
        return package.Versions.SingleOrDefault(v => v.ToString().Equals(versionLabel)) ?? throw new PackageVersionNotFoundException();
    }

    public async Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        var packagesDb = await FindAsync(Builders<PackageDbModel>.Filter.Eq(p => p.Name, packageName), cancellationToken);
        var packageDb = packagesDb.SingleOrDefault();
        return packageDb?.ToDomain();
    }

    public async Task UpdateAsync(Package package, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(Filter.Eq(p => p.Name, package.Name), new PackageDbModel(package), cancellationToken);
    }
}
