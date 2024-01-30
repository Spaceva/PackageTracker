using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Domain.Application.Model;

public abstract class ApplicationBranch
{
    public string Name { get; set; } = default!;

    public ICollection<ApplicationModule> Modules { get; set; } = new List<ApplicationModule>();

    public string? RepositoryLink { get; set; }

    public DateTime? LastCommit { get; set; }

    public IReadOnlyCollection<ApplicationPackage> PackagesWithMinVersion
    {
        get
        {
            var allPackages = Modules.SelectMany(module => module.Packages);
            var allPackagesByName = allPackages.GroupBy(package => package.PackageName);
            var packagesWithMinVersion = new List<ApplicationPackage>();
            foreach (var packages in allPackagesByName)
            {
                var minPackage = packages.MinBy(package => new PackageVersion(package.PackageVersion), new PackageVersionComparer())!;
                packagesWithMinVersion.Add(minPackage);
            }

            return packagesWithMinVersion;
        }
    }
}
