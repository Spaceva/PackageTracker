namespace PackageTracker.Scanner.AzureDevOps;

using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Infrastructure.Http;
using PackageTracker.Scanner;
using System.Threading;

internal sealed class DotNetAzureDevOpsScanner(ScannerSettings.TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<DotNetAssembly>> moduleParsers, ILogger<DotNetAzureDevOpsScanner> logger, IHttpProxy? httpProxy)
    : AzureDevOpsScanner<DotNetAssembly>(trackedApplication, mediator, moduleParsers, logger, httpProxy)
{
    private protected override Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches)
     => new DotNetApplication { Name = applicationName, Path = repositoryPath, RepositoryLink = repositoryLink, Branchs = [.. applicationBranches], RepositoryType = RepositoryType.AzureDevOps };

    private protected override ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit)
    => new DotNetApplicationBranch { Name = branchName, RepositoryLink = repositoryLink, Modules = [.. applicationModules], LastCommit = lastCommit };

    private protected override ApplicationType LookedUpApplicationType => ApplicationType.DotNet;

    private protected override async Task<IReadOnlyCollection<Model.File>> FindModuleFiles(string repositoryId, string branchId, CancellationToken cancellationToken)
    {
        var files = await AzureDevOpsClient.GetFilesAsync(repositoryId, branchId, cancellationToken);
        return files.Where(f => f.RelativePath.EndsWith(".csproj")).ToArray();
    }
}
