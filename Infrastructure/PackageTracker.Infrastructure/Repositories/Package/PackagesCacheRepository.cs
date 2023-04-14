using PackageTracker.Domain.Packages;
using PackageTracker.Domain.Packages.Exceptions;
using PackageTracker.Domain.Packages.Model;
using System.Collections.Concurrent;

namespace PackageTracker.Infrastructure.Repositories;

public class PackagesCacheRepository : CacheRepository<Package>, IPackagesRepository
{
    private readonly ConcurrentDictionary<string, Package> packages = new();

    public Task AddPackageAsync(Package package)
    {
        AddPackage(package);

        return Task.CompletedTask;
    }

    public Task AddPackagesAsync(IReadOnlyCollection<Package> packages)
    {
        foreach (var package in packages)
        {
            AddPackage(package);
        }

        return Task.CompletedTask;
    }

    public Task AddPackageVersionAsync(string packageName, PackageVersion packageVersion)
    {
        AddPackageVersion(packageName, packageVersion);

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Package>> GetAllPackagesAsync(IReadOnlyCollection<PackageType>? packageTypes = null)
        => Task.FromResult(GetAllPackages(packageTypes));

    public Task<Package> GetByNameAsync(string packageName)
     => Task.FromResult(GetByName(packageName));

    public Task<Package?> TryGetByNameAsync(string packageName)
     => Task.FromResult(TryGetByName(packageName));

    public Task<PackageVersion?> GetPackageVersionAsync(string packageName, string versionLabel)
     => Task.FromResult(GetPackageVersion(packageName, versionLabel));

    public Task<IReadOnlyCollection<PackageVersion>> GetPackageVersionsAsync(string packageName)
     => Task.FromResult(GetPackageVersions(packageName));

    public Task RemovePackageAsync(string packageName)
    {
        RemovePackage(packageName);

        return Task.CompletedTask;
    }

    public Task RemovePackageVersionAsync(string packageName, string versionLabel)
    {
        RemovePackageVersion(packageName, versionLabel);

        return Task.CompletedTask;
    }

    public Task UpdatePackageAsync(Package package)
    {
        UpdatePackage(package);

        return Task.CompletedTask;
    }

    private void AddPackage(Package package)
    {
        if (packages.ContainsKey(package.Name))
        {
            throw new PackageAlreadyExistsException();
        }

        packages.AddOrUpdate(package.Name, package, (id, oldPackage) => package ?? oldPackage);
    }

    private void AddPackageVersion(string packageName, PackageVersion packageVersion)
    {
        if (!packages.TryGetValue(packageName, out Package? package))
        {
            throw new PackageNotFoundException();
        }

        if (package.Versions.Any(v => v.Label.Equals(packageVersion.Label, StringComparison.OrdinalIgnoreCase)))
        {
            throw new PackageVersionAlreadyExistsException();
        }

        package.Versions.Add(packageVersion);
    }

    private IReadOnlyCollection<Package> GetAllPackages(IReadOnlyCollection<PackageType>? packageTypes = null)
    {
        if (packageTypes is null)
        {
            return packages.Values.ToArray();
        }

        return packages.Values.Where(p => packageTypes.Contains(p.Type)).ToArray();
    }

    private Package GetByName(string packageName)
    {
        packages.TryGetValue(packageName, out Package? package);

        if (package is null)
        {
            throw new PackageNotFoundException();
        }

        return package;
    }

    private Package? TryGetByName(string packageName)
    {
        packages.TryGetValue(packageName, out Package? package);

        return package;
    }

    private PackageVersion? GetPackageVersion(string packageName, string versionLabel)
    {
        if (!packages.TryGetValue(packageName, out Package? package))
        {
            throw new PackageNotFoundException();
        }

        return package.Versions.FirstOrDefault(v => v.Label.Equals(versionLabel, StringComparison.OrdinalIgnoreCase));
    }

    private IReadOnlyCollection<PackageVersion> GetPackageVersions(string packageName)
    {
        if (!packages.TryGetValue(packageName, out Package? package))
        {
            throw new PackageNotFoundException();
        }

        return (IReadOnlyCollection<PackageVersion>)package.Versions;
    }

    private void RemovePackage(string packageName)
    {
        if (!packages.ContainsKey(packageName))
        {
            throw new PackageNotFoundException();
        }

        packages.Remove(packageName, out _);
    }

    private void RemovePackageVersion(string packageName, string versionLabel)
    {
        if (!packages.TryGetValue(packageName, out Package? package))
        {
            throw new PackageNotFoundException();
        }

        var packageVersion = package.Versions.FirstOrDefault(v => v.Label.Equals(versionLabel, StringComparison.OrdinalIgnoreCase));
        if (packageVersion is null)
        {
            throw new PackageVersionNotFoundException();
        }

        package.Versions.Remove(packageVersion);
    }

    private void UpdatePackage(Package package)
    {
        if (!packages.ContainsKey(package.Name))
        {
            throw new PackageNotFoundException();
        }

        packages[package.Name] = package;
    }

    protected override Task<IReadOnlyCollection<Package>> GetAllAsync()
     => GetAllPackagesAsync();

    protected override Task AddAsync(IReadOnlyCollection<Package> entities)
     => AddPackagesAsync(entities);
}
