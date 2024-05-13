using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Domain.Application.Model;

public abstract class ApplicationBranch
{
    public string Name { get; set; } = default!;

    public ICollection<ApplicationModule> Modules { get; set; } = [];

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

    public static ApplicationBranch From(string branchName, string repositoryLink, IEnumerable<ApplicationModule> applicationModules, DateTime? lastCommitDate)
    {
        var applicationType = applicationModules.Select(p => p.GetType().ToApplicationType()).Max();
        var applicationBranch = (ApplicationBranch)Activator.CreateInstance(applicationType.ToApplicationBranchType())!;
        applicationBranch.Name = branchName;
        applicationBranch.RepositoryLink = repositoryLink;
        applicationBranch.Modules = [.. applicationModules];
        applicationBranch.LastCommit = lastCommitDate;
        return applicationBranch;
    }
}
