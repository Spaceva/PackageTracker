
namespace PackageTracker.Scanner.AzureDevOps.Model;
public class Project
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Url { get; init; } = default!;
    public string State { get; init; } = default!;
    public DateTime? LastUpdateTime { get; init; }
}
