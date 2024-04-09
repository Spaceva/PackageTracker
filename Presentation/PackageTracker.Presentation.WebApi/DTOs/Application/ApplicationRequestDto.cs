using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Presentation.WebApi.DTOs.Application;
internal class ApplicationRequestDto
{
    public required string Name { get; init; }

    public required string RepositoryLink { get; init; }

    public required ApplicationType Type { get; init; }
}
