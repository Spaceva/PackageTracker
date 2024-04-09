using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Presentation.WebApi.DTOs.Package;
internal class TrackPackageRequestDto
{
    public required string Name { get; init; }
    public required PackageType Type { get; init; }
}
