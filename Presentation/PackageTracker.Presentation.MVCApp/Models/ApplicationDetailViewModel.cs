using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Presentation.MVCApp.Models;
public class ApplicationDetailViewModel
{
    public string ApplicationName { get; init; } = default!;

    public string ApplicationType { get; init; } = default!;

    public string ApplicationPath { get; init; } = default!;

    public string ApplicationRepositoryLink { get; init; } = default!;

    public string ApplicationRepositoryType { get; init; } = RepositoryType.Unknown.ToString();

    public bool ApplicationIsDeadLink { get; init; }

    public bool ApplicationIsSoonDecommissioned { get; init; }

    public IReadOnlyCollection<ApplicationPackage> Packages { get; init; } = [];

    public class ApplicationPackage
    {
        public string? PackageName { get; init; }
        public string? PackageVersion { get; init; }
        public string? PackageLatestReleaseVersion { get; init; }
        public string ModuleName { get; init; } = default!;
        public string BranchName { get; init; } = default!;
        public string? BranchRepositoryLink { get; init; }
        public DateTime? BranchLastCommitDate { get; init; }
        public string MainFrameworkVersion { get; init; } = default!;
        public string MainFrameworkStatus { get; init; } = default!;
        public string? PackageType { get; init; }
        public string? PackageLink { get; init; }
        public bool IsPackageTracked { get; init; }
        public bool IsUnknownPackageVersion { get; init; }
        public bool IsFullyTracked { get; init; }
        public bool IsLatestReleaseVersion { get; init; }
        public bool CanBeUpdated { get; init; }
        public bool HasMajorVersionDelta { get; init; }
        public bool HasMinorVersionDelta { get; init; }
        public bool HasPatchVersionDelta { get; init; }

        public string UpdateColor()
        {
            if (HasMajorVersionDelta)
            {
                return "red";
            }

            if (HasMinorVersionDelta)
            {
                return "orange";
            }

            if (HasPatchVersionDelta)
            {
                return "yellow";
            }

            return "white";
        }
    }
}