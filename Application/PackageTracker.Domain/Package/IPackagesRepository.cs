namespace PackageTracker.Domain.Package;

using PackageTracker.Domain.Package.Model;

public interface IPackagesRepository
{
    Task<bool> ExistsAsync(string packageName, CancellationToken cancellationToken = default);

    Task<Package> GetByNameAsync(string packageName, CancellationToken cancellationToken = default);

    Task<Package?> TryGetByNameAsync(string packageName, CancellationToken cancellationToken = default);

    Task DeleteByNameAsync(string packageName, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Package>> GetAllAsync(string? name = null, IReadOnlyCollection<PackageType>? packageTypes = null, CancellationToken cancellationToken = default);

    Task AddAsync(Package package, CancellationToken cancellationToken = default);

    Task UpdateAsync(Package package, CancellationToken cancellationToken = default);

    Task<PackageVersion> GetVersionAsync(string packageName, string versionLabel, CancellationToken cancellationToken = default);
}
