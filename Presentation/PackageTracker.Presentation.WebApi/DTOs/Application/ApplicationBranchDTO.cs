namespace PackageTracker.Presentation.WebApi.DTOs.Application;

internal class ApplicationBranchDto
{
    public required string Name { get; init; }

    public required IReadOnlyCollection<ApplicationModuleDto> Modules { get; init; }

    public string? RepositoryLink { get; init; }

    public DateTime? LastCommit { get; init; }
}
