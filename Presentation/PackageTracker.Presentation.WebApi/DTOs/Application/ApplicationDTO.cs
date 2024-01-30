using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Presentation.WebApi.DTOs.Application;

internal class ApplicationDto : BaseDto
{
    public required string Name { get; init; } 

    public required string Path { get; init; } 

    public required string RepositoryLink { get; init; }

    public required IReadOnlyCollection<ApplicationBranchDto> Branchs { get; init; } 

    public required ApplicationType Type { get; init; }

    public required RepositoryType RepositoryType { get; init; }

    public bool IsSoonDecommissioned { get; init; }

    public bool IsDeadLink { get; init; }
}
