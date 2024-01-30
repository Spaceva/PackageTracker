namespace PackageTracker.Scanner.Gitlab;

using GitLabApiClient;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using static PackageTracker.Scanner.ScannerSettings;
using File = GitLabApiClient.Models.Files.Responses.File;

internal sealed class PhpGitlabScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<PhpModule>> phpModuleParsers, ILogger<PhpGitlabScanner> logger) 
    : GitlabScanner<PhpModule>(trackedApplication, mediator, phpModuleParsers, logger)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new PhpApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = applicationBranches.ToList(), RepositoryType = RepositoryType.Gitlab };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new PhpApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = applicationModules.ToList(), LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.Php;

    private protected override async Task<IReadOnlyCollection<File>> FindModuleFiles(int projectId, string branchName)
    {
        var tree = await GitLabClient.Trees.GetAsync(projectId, o => { o.Recursive = true; o.Reference = branchName; });
        var fileHeaders = tree.Where(t => t.Name.Equals("composer.json", StringComparison.OrdinalIgnoreCase) && !t.Path.Contains("public", StringComparison.OrdinalIgnoreCase) && !t.Path.Contains("resource", StringComparison.OrdinalIgnoreCase));
        var filesTask = fileHeaders.Select(fh => GitLabClient.Files.GetAsync(projectId, fh.Path, branchName));
        return await Task.WhenAll(filesTask);
    }
}
