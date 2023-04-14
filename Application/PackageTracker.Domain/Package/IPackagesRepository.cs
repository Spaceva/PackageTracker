namespace PackageTracker.Domain.Packages;

using PackageTracker.Domain.Packages.Model;

public interface IPackagesRepository
{
    Task<Package> GetByNameAsync(string packageName);

    Task<Package?> TryGetByNameAsync(string packageName);

    Task<IReadOnlyCollection<Package>> GetAllPackagesAsync(IReadOnlyCollection<PackageType>? packageTypes = null);

    Task AddPackageAsync(Package package);

    Task AddPackagesAsync(IReadOnlyCollection<Package> packages);

    Task RemovePackageAsync(string packageName);

    Task UpdatePackageAsync(Package package);

    Task<PackageVersion?> GetPackageVersionAsync(string packageName, string versionLabel);

    Task<IReadOnlyCollection<PackageVersion>> GetPackageVersionsAsync(string packageName);

    Task AddPackageVersionAsync(string packageName, PackageVersion packageVersion);

    Task RemovePackageVersionAsync(string packageName, string versionLabel);
}
