using PackageTracker.Scanner.AzureDevOps.Model;

namespace PackageTracker.Scanner.AzureDevOps.HttpClient.HttpResponses;

internal class RepositoriesListHttpResponse
{
    public int Count { get; init; }

    public IReadOnlyCollection<Repository> Value { get; init; } = [];
}
