using Microsoft.Extensions.Caching.Memory;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MemoryCache.Repositories;
internal class PackagesInMemoryRepository(IMemoryCache memoryCache) : IPackagesRepository
{
    public Task AddAsync(Package package, CancellationToken cancellationToken = default)
    {
        memoryCache.Set(Key(package), package);
        return Task.CompletedTask;
    }

    public Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        memoryCache.Remove(Key(packageName));
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Package>> GetAllAsync(string? name = null, IReadOnlyCollection<PackageType>? packageTypes = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<Package>>([]);
    }

    public async Task<Package> GetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        return await TryGetByNameAsync(packageName, cancellationToken) ?? throw new PackageNotFoundException();
    }

    public async Task<PackageVersion> GetVersionAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default)
    {
        var package = await GetByNameAsync(packageName, cancellationToken);
        return package.Versions.SingleOrDefault(v => v.ToString().Equals(versionLabel, StringComparison.OrdinalIgnoreCase)) ?? throw new PackageVersionNotFoundException();
    }

    public Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default)
    {
        memoryCache.TryGetValue(Key(packageName), out Package? package);
        return Task.FromResult(package);
    }

    public Task UpdateAsync(Package package, CancellationToken cancellationToken = default)
    {
        memoryCache.Set(Key(package), package);
        return Task.CompletedTask;
    }

    private static string Key(Package package)
     => Key(package.Name);

    private static string Key(string packageName)
     => $"{nameof(PackagesInMemoryRepository)}-{packageName}";
}
