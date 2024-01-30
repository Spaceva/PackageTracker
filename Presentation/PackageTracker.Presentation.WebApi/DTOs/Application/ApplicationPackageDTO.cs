using PackageTracker.Presentation.WebApi.DTOs.Package;

namespace PackageTracker.Presentation.WebApi.DTOs.Application;

internal class ApplicationPackageDto
{
    public required string PackageName { get; init; }

    public required string PackageVersion { get; init; }

    public PackageDto? TrackedPackage { get; init; }

    public PackageVersionDto? TrackedPackageVersion { get; init; }

    public bool IsPackageTracked { get; init; }

    public bool IsUnknownPackageVersion { get; init; }

    public bool IsFullyTracked { get; init; }

    public bool IsLatestReleaseVersion { get; init; }

    public bool CanBeUpdated { get; init; }
}
