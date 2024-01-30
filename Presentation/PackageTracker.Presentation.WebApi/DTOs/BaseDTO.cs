namespace PackageTracker.Presentation.WebApi.DTOs;
internal abstract class BaseDto
{
    public IReadOnlyCollection<LinkDto> Links { get; internal set; } = [];
}
