using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Scanner.GitHub;

internal abstract class GitHubScannerRegistrator : IScannerRegistrator
{
    public IApplicationsScanner Register(IServiceProvider serviceProvider, ScannerSettings scannerSettings, ScannerSettings.TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> parsers, IMediator mediator)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<GitHubScanner>();
        return new GitHubScanner(GetRepositoriesAsync, trackedApplication, parsers, logger, mediator);
    }

    protected abstract Task<IReadOnlyList<Repository>> GetRepositoriesAsync(IGitHubClient gitHubClient, string name);
}
