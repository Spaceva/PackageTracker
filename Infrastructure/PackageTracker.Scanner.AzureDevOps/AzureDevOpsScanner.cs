using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure.Http;
using PackageTracker.Messages.Queries;
using PackageTracker.Scanner.AzureDevOps.Model;
using System.Collections.Concurrent;
using static PackageTracker.Scanner.ScannerSettings;
using DownloadedFile = (string Name, string Content);

namespace PackageTracker.Scanner.AzureDevOps;

internal class AzureDevOpsScanner(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IHttpProxy? httpProxy) : IApplicationsScanner
{
    private AzureDevOpsHttpClient? azureDevOpsClient;

    private AzureDevOpsHttpClient AzureDevOpsClient
    {
        get
        {
            azureDevOpsClient ??= new(trackedApplication.RepositoryRootLink, trackedApplication.AccessToken, httpProxy);
            return azureDevOpsClient;
        }
    }

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { RepositoryTypes = [RepositoryType.AzureDevOps], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await AzureDevOpsClient.ListRepositoriesAsync(cancellationToken);
        var remoteApplications = repositories.Select(repository => new UntypedApplication { Name = repository.Name, Path = repository.Project.Name, RepositoryLink = repository.WebUrl, Branchs = [], RepositoryType = RepositoryType.AzureDevOps }).ToArray();

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
    }

    public async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var projectsInformation = new ConcurrentBag<Application>();
        var repositories = await AzureDevOpsClient.ListRepositoriesAsync(cancellationToken);
        try
        {
            await Parallel.ForEachAsync(repositories, cancellationToken, async (repository, cancellationToken) =>
            {
                logger.LogDebug("Scanning {ScannerType} Repository '{RepositoryName}' ...", RepositoryType.AzureDevOps, repository.Name);
                var application = await ScanRepositoryAsync(repository, cancellationToken);
                if (application is not null)
                {
                    projectsInformation.Add(application);
                }
            });
        }
        catch (TaskCanceledException)
        {
            logger.LogWarning("Operation cancelled.");
            return [];
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled.");
            return [];
        }

        return projectsInformation;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        AzureDevOpsClient.Dispose();
    }

    private protected async Task<IReadOnlyCollection<RepositoryBranch>> FindAllLongTermBranchs(string repositoryId, CancellationToken cancellationToken)
    {
        var branches = await AzureDevOpsClient.ListRepositoryBranchsAsync(repositoryId, cancellationToken);
        return branches.Where(b => Scanner.Constants.Git.ValidBranches.Contains(b.Name.Split('/')[^1])).ToArray();
    }

    private async Task<Application?> ScanRepositoryAsync(Repository repository, CancellationToken cancellationToken)
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
                var moduleParser = moduleParsers.FirstOrDefault(mp => Array.TrueForAll(downloadedFiles, f => mp.CanParse(f.Content)));
                if (moduleParser is null)
                {
                    return null;
                }

                var moduleParsingTask = downloadedFiles.Select(f => moduleParser.ParseModuleAsync(f.Content, f.Name, cancellationToken));
                var modules = await Task.WhenAll(moduleParsingTask);

                var branchName = branch.Name.Split('/')[^1];
                var lastCommitDate = await AzureDevOpsClient.GetLastCommitAsync(repository.Id, branchName, cancellationToken);

                applicationBranchs.Add(ApplicationBranch.From(branchName, branch.Url, modules, lastCommitDate));
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
            logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private async Task<DownloadedFile[]> DownloadFilesAsync(Repository repository, RepositoryBranch branch, IEnumerable<Model.File> moduleFilesMetadata, CancellationToken cancellationToken)
    {
        var fileContentTasks = moduleFilesMetadata.Select(mf => AzureDevOpsClient.GetFileContentAsync(repository.Id, branch.ObjectId, mf.ObjectId, cancellationToken).ContinueWith(task => new DownloadedFile(Path.GetFileName(mf.RelativePath), task.Result)));
        var downloadedFiles = await Task.WhenAll(fileContentTasks);
        return downloadedFiles;
    }

    private IEnumerable<Model.File> FindModuleFiles(IEnumerable<Model.File> files)
    {
        return files.Where(f => moduleParsers.Any(p => p.IsModuleFile(f.RelativePath)));
    }
}
