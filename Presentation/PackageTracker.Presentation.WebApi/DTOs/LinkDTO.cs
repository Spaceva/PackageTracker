namespace PackageTracker.Presentation.WebApi.DTOs;

internal class LinkDto
{
    public required string Rel { get; init; }

    public required string Href { get; init; }

    public required string Method { get; init; }
}
