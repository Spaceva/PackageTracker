using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Infrastructure.Http;
using PackageTracker.Scanner.AzureDevOps.Model;
using static PackageTracker.Scanner.ScannerSettings;
using DownloadedFile = (string Name, string Content);

namespace PackageTracker.Scanner.AzureDevOps;

internal class AzureDevOpsScanner(IHttpProxy? httpProxy, TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IMediator mediator)
    : BaseScanner<Repository>(trackedApplication, moduleParsers, logger, mediator)
{
    private AzureDevOpsHttpClient? azureDevOpsClient;
    private AzureDevOpsHttpClient AzureDevOpsClient
    {
        get
        {
            azureDevOpsClient ??= new(TrackedApplication.RepositoryRootLink, TrackedApplication.AccessToken, httpProxy);
            return azureDevOpsClient;
        }
    }

    protected override RepositoryType RepositoryType => RepositoryType.AzureDevOps;

    protected override UntypedApplication AsUntypedApplication(Repository repository)
     => new() { Name = repository.Name, Path = repository.Project.Name, RepositoryLink = repository.WebUrl, Branchs = [], RepositoryType = RepositoryType.AzureDevOps };

    protected override string BranchLinkSuffix(string branchName) => string.Empty;

    protected override Task CheckTokenExpirationAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override void Dispose(bool isDisposing)
    {
        AzureDevOpsClient.Dispose();
    }

    protected override async Task<IEnumerable<Repository>> GetRepositoriesAsync(CancellationToken cancellationToken) => await AzureDevOpsClient.ListRepositoriesAsync(cancellationToken);

    protected override bool IsNotArchived(Repository repository) => !repository.IsDisabled && !repository.IsInMaintenance;

    protected override string NameOf(Repository repository) => repository.Name;

    protected override async Task<Application?> ScanRepositoryAsync(Repository repository, CancellationToken cancellationToken)
    {
        try
        {
            var branchs = await FindAllLongTermBranchs(repository.Id, cancellationToken);
            var applicationBranchs = new List<ApplicationBranch>();

            foreach (var branch in branchs)
            {
                var filesMetadata = await AzureDevOpsClient.GetFilesAsync(repository.Id, branch.ObjectId, cancellationToken);
                var moduleFilesMetadata = FindModuleFiles(filesMetadata);
                if (!moduleFilesMetadata.Any())
                {
                    return null;
                }

                var downloadedFiles = await DownloadFilesAsync(repository, branch, moduleFilesMetadata, cancellationToken);
                var moduleParser = ModuleParsers.FirstOrDefault(mp => Array.TrueForAll(downloadedFiles, f => mp.CanParse(f.Content)));
                if (moduleParser is null)
                {
                    return null;
                }

                var moduleParsingTask = downloadedFiles.Select(f => moduleParser.ParseModuleAsync(f.Content, f.Name, cancellationToken));
                var modules = await Task.WhenAll(moduleParsingTask);

                var branchName = branch.Name.Split('/')[^1];
                var lastCommitDate = await AzureDevOpsClient.GetLastCommitAsync(repository.Id, branchName, cancellationToken);

                applicationBranchs.Add(ApplicationBranch.From(branchName, branch.Url + BranchLinkSuffix(branch.Name), modules, lastCommitDate));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application.From(repository.Name, repository.Project.Name, repository.WebUrl, applicationBranchs, RepositoryType.AzureDevOps);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private async Task<IReadOnlyCollection<RepositoryBranch>> FindAllLongTermBranchs(string repositoryId, CancellationToken cancellationToken)
    {
        var branches = await AzureDevOpsClient.ListRepositoryBranchsAsync(repositoryId, cancellationToken);
        return [.. branches.Where(b => Scanner.Constants.Git.ValidBranches.Contains(b.Name.Split('/')[^1]))];
    }

    private async Task<DownloadedFile[]> DownloadFilesAsync(Repository repository, RepositoryBranch branch, IEnumerable<Model.File> moduleFilesMetadata, CancellationToken cancellationToken)
    {
        var fileContentTasks = moduleFilesMetadata.Select(mf => AzureDevOpsClient.GetFileContentAsync(repository.Id, branch.ObjectId, mf.ObjectId, cancellationToken).ContinueWith(task => new DownloadedFile(Path.GetFileName(mf.RelativePath), task.Result)));
        var downloadedFiles = await Task.WhenAll(fileContentTasks);
        return downloadedFiles;
    }

    private IEnumerable<Model.File> FindModuleFiles(IEnumerable<Model.File> files)
    {
        return files.Where(f => ModuleParsers.Any(p => p.IsModuleFile(f.RelativePath)));
    }
}
