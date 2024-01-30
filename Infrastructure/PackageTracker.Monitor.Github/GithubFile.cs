namespace PackageTracker.Monitor.Github;
internal class GithubFile
{
    public string Name { get; init; } = default!;
    public string Content { get; init; } = default!;
    public string Encoding { get; init; } = default!;
}