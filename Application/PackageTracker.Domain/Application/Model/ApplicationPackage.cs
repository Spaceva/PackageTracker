namespace PackageTracker.Domain.Application.Model;
public class ApplicationPackage
{
    public string PackageName { get; set; } = default!;

    public string PackageVersion { get; set; } = default!;

    public Package.Model.Package? TrackedPackage { get; set; }

    public Package.Model.PackageVersion? TrackedPackageVersion { get; set; }

    public bool IsPackageTracked => TrackedPackage is not null;

    public bool IsUnknownPackageVersion => IsPackageTracked && TrackedPackageVersion is null;

    public bool IsFullyTracked => IsPackageTracked && TrackedPackageVersion is not null;

    public bool IsLatestReleaseVersion => IsFullyTracked && TrackedPackage!.LatestReleaseVersion is not null && TrackedPackageVersion!.Equals(TrackedPackage.LatestReleaseVersion);

    public bool CanBeUpdated => IsFullyTracked && TrackedPackage!.LatestReleaseVersion is not null && !TrackedPackageVersion!.Equals(TrackedPackage.LatestReleaseVersion);
}
