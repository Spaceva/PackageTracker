using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Presentation.WebApi.DTOs.Package;

internal class PackageDto : BaseDto
{
    public required string Name { get; init; }

    public required PackageType Type { get; init; }

    public required string RegistryUrl { get; init; } 

    public required string Link { get; init; }

    public string? LatestReleaseVersion { get; init; }

    public string? LatestVersion { get; init; }
}
