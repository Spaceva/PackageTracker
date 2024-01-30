namespace PackageTracker.Presentation.WebApi.DTOs.Package;

internal class PackageVersionDto
{
    public required string Value { get; init; }

    public required bool IsRelease { get; init; }

    public required bool IsPreRelease { get; init; }
}
