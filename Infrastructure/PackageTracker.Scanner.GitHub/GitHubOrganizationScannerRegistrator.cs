using Octokit;

namespace PackageTracker.Scanner.GitHub;
internal class GitHubOrganizationScannerRegistrator : GitHubScannerRegistrator
{
    protected override Task<IReadOnlyList<Repository>> GetRepositoriesAsync(IGitHubClient gitHubClient, string name)
     => gitHubClient.Repository.GetAllForOrg(name);
}
