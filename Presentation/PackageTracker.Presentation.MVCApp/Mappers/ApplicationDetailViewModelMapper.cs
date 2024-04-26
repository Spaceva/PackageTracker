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
            Packages = application.Branchs.OrderBy(branch => branch.Name).SelectMany(branch => MapToApplicationPackageRow(branch)).ToArray()
        };
    }

    private static ApplicationDetailViewModel.ApplicationPackage[] MapToApplicationPackageRow(ApplicationBranch branch)
    {
        var rows = new List<ApplicationDetailViewModel.ApplicationPackage>();
        foreach (var module in branch.Modules.OrderBy(m => m.Name))
        {
            var packageType = PackageType.Npm;
            var mainFrameworkVersion = "N/A";
            if (module is AngularModule angularModule)
            {
                mainFrameworkVersion = $"Angular v{angularModule.FrameworkVersion}";
            }
            else if (module is DotNetAssembly dotNetAssembly)
            {
                packageType = PackageType.Nuget;
                mainFrameworkVersion = $".NET {dotNetAssembly.FrameworkVersion}";
            }
            else if (module is PhpModule phpModule)
            {
                packageType = PackageType.Packagist;
                mainFrameworkVersion = $"PHP {phpModule.FrameworkVersion}";
            }
            else if (module is JavaModule javaModule)
            {
                packageType = PackageType.Java;
                mainFrameworkVersion = $"JAVA {javaModule.FrameworkVersion}";
            }

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

                rows.Add(row);
                continue;
            }
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
                    PackageType = packageType.ToString(),
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
        }
        return [.. rows];
    }
}
