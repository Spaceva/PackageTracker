namespace PackageTracker.Presentation.MVCApp.Models;

public class PackageViewModel
{
    public string Name { get; init; } = default!;

    public string? LatestVersion { get; init; } = default!;

    public string? LatestReleaseVersion { get; init; }

    public string Type { get; init; } = default!;

    public string? Link { get; init; }
}
