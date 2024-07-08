using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Presentation.MVCApp.Models;

namespace PackageTracker.Presentation.MVCApp.Mappers;

internal static class ApplicationDetailViewModelMapper
{
    public static ApplicationDetailViewModel ToApplicationDetailViewModel(this Application application)

    {
        return new ApplicationDetailViewModel()
        {
            ApplicationName = application.Name,
            ApplicationPath = application.Path,
            ApplicationType = application.Type.ToString(),
            ApplicationRepositoryLink = application.RepositoryLink,
            ApplicationRepositoryType = application.RepositoryType.ToString(),
            ApplicationIsSoonDecommissioned = application.IsSoonDecommissioned,
            ApplicationIsDeadLink = application.IsDeadLink,
            Packages = [.. application.Branchs.OrderBy(branch => branch.Name).SelectMany(MapToApplicationPackageRow)]
        };
    }

    private static List<ApplicationDetailViewModel.ApplicationPackage> MapToApplicationPackageRow(ApplicationBranch branch)
    {
        var rows = new List<ApplicationDetailViewModel.ApplicationPackage>();
        foreach (var module in branch.Modules.OrderBy(m => m.Name))
        {
            rows.AddRange(MapToApplicationPackageRow(branch, module));
        }

        return rows;
    }

    private static List<ApplicationDetailViewModel.ApplicationPackage> MapToApplicationPackageRow(ApplicationBranch branch, ApplicationModule module)
    {
        var metadata = module.ParseMetadata();
        var mainFrameworkVersion = metadata.FrameworkVersion;
        var mainFrameworkStatus = module.Framework?.Status.ToString() ?? "Unknown";

        if (module.Packages.Count == 0)
        {
            var row = new ApplicationDetailViewModel.ApplicationPackage
            {
                BranchName = branch.Name,
                BranchRepositoryLink = branch.RepositoryLink,
                BranchLastCommitDate = branch.LastCommit,
                ModuleName = module.Name,
                MainFrameworkVersion = mainFrameworkVersion,
                MainFrameworkStatus = mainFrameworkStatus,
            };

            return [row];
        }

        var rows = new List<ApplicationDetailViewModel.ApplicationPackage>();
        foreach (var package in module.Packages.OrderBy(ap => ap.PackageName))
        {
            var currentVersion = new PackageVersion(package.PackageVersion);
            var latestReleaseVersion = package.TrackedPackage?.LatestReleaseVersion is not null ? new PackageVersion(package.TrackedPackage.LatestReleaseVersion) : null;
            var row = new ApplicationDetailViewModel.ApplicationPackage
            {
                BranchName = branch.Name,
                BranchRepositoryLink = branch.RepositoryLink,
                BranchLastCommitDate = branch.LastCommit,
                ModuleName = module.Name,
                PackageName = package.PackageName,
                PackageType = metadata.PackageType,
                MainFrameworkVersion = mainFrameworkVersion,
                MainFrameworkStatus = mainFrameworkStatus,
                PackageVersion = package.PackageVersion,
                PackageLatestReleaseVersion = latestReleaseVersion?.ToString(),
                PackageLink = package.TrackedPackage?.Link,
                CanBeUpdated = package.CanBeUpdated,
                IsFullyTracked = package.IsFullyTracked,
                IsLatestReleaseVersion = package.IsLatestReleaseVersion,
                IsPackageTracked = package.IsPackageTracked,
                IsUnknownPackageVersion = package.IsUnknownPackageVersion,
                HasMajorVersionDelta = latestReleaseVersion is not null && latestReleaseVersion.Major != currentVersion.Major,
                HasMinorVersionDelta = latestReleaseVersion is not null
                                       && latestReleaseVersion.Major == currentVersion.Major
                                       && latestReleaseVersion.Minor != currentVersion.Minor,
                HasPatchVersionDelta = latestReleaseVersion is not null
                                       && latestReleaseVersion.Major == currentVersion.Major
                                       && latestReleaseVersion.Minor == currentVersion.Minor
                                       && latestReleaseVersion.Patch != currentVersion.Patch,
            };

            rows.Add(row);
        }
        return rows;
    }

    private static (string FrameworkVersion, string PackageType) ParseMetadata(this ApplicationModule module)
    {
        if (module is AngularModule angularModule)
        {
            var frameworkVersion = $"Angular v{angularModule.FrameworkVersion}";
            return (frameworkVersion, PackageType.Npm.ToString());
        }

        if (module is ReactModule reactModule)
        {
            var frameworkVersion = $"React v{reactModule.FrameworkVersion}";
            return (frameworkVersion, PackageType.Npm.ToString());
        }

        if (module is DotNetAssembly dotNetAssembly)
        {
            var frameworkVersion = $".NET {dotNetAssembly.FrameworkVersion}";
            return (frameworkVersion, PackageType.Nuget.ToString());
        }

        if (module is PhpModule phpModule)
        {
            var frameworkVersion = $"PHP {phpModule.FrameworkVersion}";
            return (frameworkVersion, PackageType.Packagist.ToString());
        }

        if (module is JavaModule javaModule)
        {
            var frameworkVersion = $"JAVA {javaModule.FrameworkVersion}";
            return (frameworkVersion, PackageType.Java.ToString());
        }

        throw new ArgumentOutOfRangeException(nameof(module), "Unknown module type.");
    }
}
