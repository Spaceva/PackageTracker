namespace PackageTracker.Presentation.WebApi.DTOs.Application;

using PackageTracker.Presentation.WebApi.DTOs.Framework;

internal class ApplicationModuleDto
{
    public required string Name { get; init; }

    public required ICollection<ApplicationPackageDto> Packages { get; init; }

    public FrameworkDto? Framework { get; init; }
}
