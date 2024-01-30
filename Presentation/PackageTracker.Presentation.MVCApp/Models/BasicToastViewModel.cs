namespace PackageTracker.Presentation.MVCApp.Models;

public class BasicToastViewModel
{
    public string Title { get; init; } = default!;

    public string Message { get; init; } = default!;

    public string Id { get; init; } = default!;

    public string CSSClass { get; init; } = "info";
}
