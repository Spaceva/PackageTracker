using PackageTracker.Scanner.AzureDevOps.Model;

namespace PackageTracker.Scanner.AzureDevOps.HttpClient.HttpResponses;

internal class RepositoryBranchsListHttpResponse
{
    public int Count { get; init; }

    public IReadOnlyCollection<RepositoryBranch> Value { get; init; } = Array.Empty<RepositoryBranch>();
}
