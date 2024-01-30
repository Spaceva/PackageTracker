namespace PackageTracker.Presentation.WebApi.DTOs.Package;

internal class PackageWithVersionsDto : PackageDto
{
    public required IReadOnlyCollection<PackageVersionDto> Versions { get; init; }
}
