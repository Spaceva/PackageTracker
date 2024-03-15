namespace PackageTracker.Scanner.GitHub;

using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;
using Application = Domain.Application.Model.Application;
using RepositoryType = Domain.Application.Model.RepositoryType;

internal sealed class AngularGitHubScanner(Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate, TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<AngularModule>> angularModuleParsers, ILogger<AngularGitHubScanner> logger) 
    : GitHubScanner<AngularModule>(getRepositoriesDelegate, trackedApplication, mediator, angularModuleParsers, logger)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new AngularApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = [.. applicationBranches], RepositoryType = RepositoryType.GitHub };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new AngularApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = [.. applicationModules], LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.Angular;

    protected override bool TreeItemMatchPattern(TreeItem item)
     => item.Path.EndsWith("package.json", StringComparison.OrdinalIgnoreCase)
        && !item.Path.Contains("public", StringComparison.OrdinalIgnoreCase)
        && !item.Path.Contains("resource", StringComparison.OrdinalIgnoreCase);
}
