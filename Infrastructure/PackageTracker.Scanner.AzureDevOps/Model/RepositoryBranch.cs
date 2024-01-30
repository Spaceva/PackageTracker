namespace PackageTracker.Scanner.AzureDevOps.Model;
internal class RepositoryBranch
{
    public string Name { get; init; } = default!;
    public string ObjectId { get; init; } = default!;
    public string Url { get; init; } = default!;
}
