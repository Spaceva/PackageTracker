using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner.GitHub;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddGitHubUserScanner(this IScannerRegistrator services, string trackerName)
        => AddGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForUser(name));
    public static IScannerRegistrator AddGitHubOrganizationScanner(this IScannerRegistrator services, string trackerName)
        => AddGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForOrg(name));

    private static IScannerRegistrator AddGitHubScanner(this IScannerRegistrator services, string trackerName, Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase)) ?? throw new UnknownScannerException();
            var parsers = sp.GetServices<IApplicationModuleParser>();
            return new GitHubScanner(getRepositoriesDelegate, trackedApplication, mediator, parsers, loggerFactory.CreateLogger<GitHubScanner>());
        });
}
