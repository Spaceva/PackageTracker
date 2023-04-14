namespace PackageTracker.Messages.Queries;

public class GetPackageByNameQueryResponse
{
    public string Name { get; init; } = default!;

    public string LatestVersion { get; init; } = default!;

    public string? LatestReleaseVersion { get; init; }

    public string Type { get; init; } = default!;

    public string? Link { get; init; }

    public IReadOnlyCollection<string> Versions { get; init; } = Array.Empty<string>();
}