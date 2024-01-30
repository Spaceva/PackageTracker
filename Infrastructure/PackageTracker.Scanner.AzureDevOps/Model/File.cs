namespace PackageTracker.Scanner.AzureDevOps.Model;

internal class File
{
    public string ObjectId { get; init; } = default!;
    public string RelativePath { get; init; } = default!;
    public string Mode { get; init; } = default!;
    public string GitObjectType { get; init; } = default!;
    public string Url { get; init; } = default!;
    public int Size { get; init; }
}
