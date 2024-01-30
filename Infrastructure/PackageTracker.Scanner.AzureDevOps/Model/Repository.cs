namespace PackageTracker.Scanner.AzureDevOps.Model;

internal class Repository
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string WebUrl { get; init; } = default!;
    public Project Project { get; init; } = default!;
    public bool IsDisabled { get; init; }
    public bool IsInMaintenance { get; init; }
}
