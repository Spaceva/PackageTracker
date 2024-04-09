namespace PackageTracker.Scanner.AzureDevOps.HttpClient.HttpResponses;
internal class TreeListHttpResponse
{
    public string ObjectId { get; init; } = default!;

    public string Url { get; init; } = default!;

    public IReadOnlyCollection<Model.File> TreeEntries { get; init; } = [];
}
