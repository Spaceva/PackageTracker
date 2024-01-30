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

namespace PackageTracker.Scanner.AzureDevOps;

internal abstract class AzureDevOpsScanner<TApplicationModule>(TrackedApplication trackedApplication, IMediator mediator, IEnumerable<IApplicationModuleParser<TApplicationModule>> moduleParsers, ILogger logger, IHttpProxy? httpProxy) : IApplicationsScanner where TApplicationModule : ApplicationModule
{
    private protected ILogger Logger => logger;

    private protected AzureDevOpsHttpClient AzureDevOpsClient => new(trackedApplication.RepositoryRootLink, trackedApplication.AccessToken, httpProxy);

    private protected IEnumerable<IApplicationModuleParser<TApplicationModule>> ModuleParsers => moduleParsers;

    public async Task<IReadOnlyCollection<Application>> ScanRemoteAsync(CancellationToken cancellationToken)
    {
        var projectsInformation = new ConcurrentBag<Application>();
        var repositories = await AzureDevOpsClient.ListRepositoriesAsync(cancellationToken);
        try
        {
            await Parallel.ForEachAsync(repositories, cancellationToken, async (repository, cancellationToken) =>
            {
                var application = await ScanRepositoryAsync(repository, cancellationToken);
                if (application is not null)
                {
                    projectsInformation.Add(application);
                }
            });
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
            return [];
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
            return [];
        }

        return projectsInformation;
    }

    public async Task<IReadOnlyCollection<Application>> FindDeadLinksAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery { SearchCriteria = new ApplicationSearchCriteria { ApplicationTypes = [LookedUpApplicationType], RepositoryTypes = [RepositoryType.AzureDevOps], ShowDeadLink = true } }, cancellationToken);
        var localApplications = response.Applications;

        var repositories = await AzureDevOpsClient.ListRepositoriesAsync(cancellationToken);
        var remoteApplications = repositories.Select(repository => Application(repository.Name, repository.Project.Name, repository.WebUrl, [])).ToArray();

        var comparer = new ApplicationBasicComparer();
        return [.. localApplications.Where(app => !remoteApplications.Contains(app, comparer))];
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

    private protected abstract Application Application(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches);

    private protected abstract ApplicationBranch ApplicationBranch(string branchName, string repositoryLink, IReadOnlyCollection<ApplicationModule> applicationModules, DateTime? lastCommit);

    private protected abstract Task<IReadOnlyCollection<Model.File>> FindModuleFiles(string repositoryId, string branchId, CancellationToken cancellationToken);

    private protected abstract ApplicationType LookedUpApplicationType { get; }

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
                var moduleFiles = await FindModuleFiles(repository.Id, branch.ObjectId, cancellationToken);
                if (moduleFiles.Count == 0)
                {
                    return null;
                }

                var fileContentTasks = moduleFiles.Select(mf => AzureDevOpsClient.GetFileContentAsync(repository.Id, branch.ObjectId, mf.ObjectId, cancellationToken).ContinueWith(task => new { Name = Path.GetFileName(mf.RelativePath), Content = task.Result }));
                var files = await Task.WhenAll(fileContentTasks);
                var moduleParser = ModuleParsers.FirstOrDefault(mp => Array.TrueForAll(files, f => mp.CanParse(f.Content)));
                if (moduleParser is null)
                {
                    return null;
                }
                var moduleParsingTask = files.Select(f => moduleParser.ParseModuleAsync(f.Content, f.Name, cancellationToken));
                var modules = await Task.WhenAll(moduleParsingTask);

                var branchName = branch.Name.Split('/')[^1];
                var lastCommitDate = await AzureDevOpsClient.GetLastCommitAsync(repository.Id, branchName, cancellationToken);

                applicationBranchs.Add(ApplicationBranch(branchName, branch.Url, modules, lastCommitDate));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application(repository.Name, repository.Project.Name, repository.WebUrl, applicationBranchs);
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
}
