﻿namespace PackageTracker.Scanner.Gitlab;

using GitLabApiClient.Models.Trees.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;

internal sealed class DotNetGitlabScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<DotNetAssembly>> dotNetAssemblyParsers, ILogger<DotNetGitlabScanner> logger)
    : GitlabScanner<DotNetAssembly>(trackedApplication, mediator, dotNetAssemblyParsers, logger)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new DotNetApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = [.. applicationBranches], RepositoryType = RepositoryType.Gitlab };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new DotNetApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = [.. applicationModules], LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.DotNet;

    private protected override bool TreeItemMatchPattern(Tree tree)
     => tree.Name.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase);
}
