﻿namespace PackageTracker.Scanner.Gitlab;

using GitLabApiClient.Models.Trees.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;

internal sealed class AngularGitlabScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<AngularModule>> angularModuleParsers, ILogger<AngularGitlabScanner> logger) 
    : GitlabScanner<AngularModule>(trackedApplication, mediator, angularModuleParsers, logger)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new AngularApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = [.. applicationBranches], RepositoryType = RepositoryType.Gitlab };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new AngularApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = [.. applicationModules], LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.Angular;

    private protected override bool TreeItemMatchPattern(Tree tree)
     => tree.Name.Equals("package.json", StringComparison.OrdinalIgnoreCase)
        && !tree.Path.Contains("public", StringComparison.OrdinalIgnoreCase)
        && !tree.Path.Contains("resource", StringComparison.OrdinalIgnoreCase);
}
