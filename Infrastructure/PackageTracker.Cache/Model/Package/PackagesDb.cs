using PackageTracker.Domain.Packages.Model;

namespace PackageTracker.Cache;

internal class PackagesDb
{
    public IReadOnlyCollection<NpmPackage> NpmPackages { get; set; } = default!;

    public IReadOnlyCollection<NugetPackage> NugetPackages { get; set; } = default!;
}
