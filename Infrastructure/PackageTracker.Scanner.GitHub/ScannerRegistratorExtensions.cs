using Octokit;

namespace PackageTracker.Scanner.GitHub;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddGitHubUserScanner(this IScannerRegistrator services, string trackerName)
        => AddGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForUser(name));
    public static IScannerRegistrator AddGitHubOrganizationScanner(this IScannerRegistrator services, string trackerName)
        => AddGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForOrg(name));

    private static IScannerRegistrator AddGitHubScanner(this IScannerRegistrator services, string trackerName, Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate)
    => services.Register<GitHubScanner>(trackerName, (sp, settings, trackedApplication, parsers, logger, mediator) => new GitHubScanner(getRepositoriesDelegate, trackedApplication, parsers, logger, mediator));
}
