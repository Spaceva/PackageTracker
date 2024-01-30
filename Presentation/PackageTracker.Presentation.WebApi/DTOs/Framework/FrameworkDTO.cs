using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Presentation.WebApi.DTOs.Framework;

internal class FrameworkDto : BaseDto
{
    public required string Name { get; init; } 

    public required string Version { get; init; } 

    public required string Channel { get; init; }

    public string? CodeName { get; init; }

    public FrameworkStatus Status { get; init; }

    public DateTime? ReleaseDate { get; init; }

    public DateTime? EndOfLife { get; init; }

    public bool IsEnded { get; init; }
}