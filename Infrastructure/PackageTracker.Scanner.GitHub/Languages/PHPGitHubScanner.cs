namespace PackageTracker.Scanner.GitHub;

using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;
using Application = Domain.Application.Model.Application;
using RepositoryType = Domain.Application.Model.RepositoryType;

internal sealed class PhpGitHubScanner(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser> phpModuleParsers, ILogger<PhpGitHubScanner> logger)
    : GitHubScanner<PhpModule>(getRepositoriesDelegate, trackedApplication, mediator, phpModuleParsers, logger)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new PhpApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = [.. applicationBranches], RepositoryType = RepositoryType.GitHub };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new PhpApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = [.. applicationModules], LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.Php;
}
