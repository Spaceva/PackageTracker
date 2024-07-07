using Octokit;

namespace PackageTracker.Scanner.GitHub;
internal class GitHubUserScannerRegistrator : GitHubScannerRegistrator
{
    protected override Task<IReadOnlyList<Repository>> GetRepositoriesAsync(IGitHubClient gitHubClient, string name)
     => gitHubClient.Repository.GetAllForUser(name);
}
