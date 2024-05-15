using GitLabApiClient;
using GitLabApiClient.Models.Branches.Responses;
using GitLabApiClient.Models.Projects.Responses;
using GitLabApiClient.Models.Users.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using System.Globalization;
using System.Net.Http.Json;
using static PackageTracker.Scanner.ScannerSettings;
using File = GitLabApiClient.Models.Files.Responses.File;

namespace PackageTracker.Scanner.Gitlab;

internal class GitlabScanner(TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> moduleParsers, ILogger logger, IMediator mediator)
    : BaseScanner<Project>(trackedApplication, moduleParsers, logger, mediator)
{
    private const string PUBLIC_HOST = "https://gitlab.com";

    private static readonly TimeSpan DefaultTokenExpirationWarningThreshold = TimeSpan.FromDays(7);

    private TimeSpan TokenExpirationWarningThreshold => TrackedApplication.TokenExpirationWarningThreshold ?? DefaultTokenExpirationWarningThreshold;

    private string? userName;

    private GitLabClient? gitLabClient;
    private GitLabClient GitLabClient
    {
        get
        {
            if (gitLabClient is null)
            {
                if (TrackedApplication.RepositoryRootLink.StartsWith(PUBLIC_HOST))
                {
                    gitLabClient = new(PUBLIC_HOST, TrackedApplication.AccessToken);
                    var uri = new Uri(TrackedApplication.RepositoryRootLink);
                    userName = uri.LocalPath.Replace("/", string.Empty);
                }
                else
                {
                    gitLabClient = new(TrackedApplication.RepositoryRootLink, TrackedApplication.AccessToken);
                    userName = string.Empty;
                }
            }

            return gitLabClient;
        }
    }

    private HttpClient? httpClient;
    private HttpClient HttpClient
    {
        get
        {
            if (httpClient is null)
            {
                httpClient = new() { BaseAddress = new Uri(TrackedApplication.RepositoryRootLink), };
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + TrackedApplication.AccessToken);
            }

            return httpClient;
        }
    }

    protected override RepositoryType RepositoryType => RepositoryType.Gitlab;

    protected override UntypedApplication AsUntypedApplication(Project repository)
    => new() { Name = repository.Name, Path = repository.Namespace.FullPath.Replace("/", ">"), RepositoryLink = repository.HttpUrlToRepo.Replace(".git", "/"), Branchs = [], RepositoryType = RepositoryType.Gitlab };

    protected override string BranchLinkSuffix(string branchName) => $"-/tree/{branchName}";

    protected override async Task CheckTokenExpirationAsync(CancellationToken cancellationToken)
    {
        var tokenInfo = await HttpClient.GetFromJsonAsync<PersonnalAccessToken>("api/v4/personal_access_tokens/self", cancellationToken);
        if (tokenInfo is not { Active: true, Revoked: false })
        {
            throw new GitLabException(System.Net.HttpStatusCode.Unauthorized, "Token is inactive or revoked.");
        }

        var tokenLifeSpan = tokenInfo.ExpiresAt.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow;
        if (tokenLifeSpan < TokenExpirationWarningThreshold)
        {
            Logger.LogWarning("Token is expiring soon : {TokenLifeSpan}", tokenLifeSpan.ToString(@"dd\:hh\:mm\:ss"));
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        HttpClient.Dispose();
    }

    protected override async Task<IEnumerable<Project>> GetRepositoriesAsync(CancellationToken cancellationToken)
        => await GitLabClient.Projects.GetAsync(opt =>
         {
             opt.Simple = true;
             if (string.IsNullOrWhiteSpace(userName))
             {
                 return;
             }

             opt.UserId = userName;
         });

    protected override bool IsNotArchived(Project repository) => !repository.Archived;

    protected override string NameOf(Project repository) => repository.Name;

    protected override async Task<Application?> ScanRepositoryAsync(Project repository, CancellationToken cancellationToken)
    {
        try
        {
            var branchs = await FindAllLongTermBranchs(repository.Id);

            var applicationBranchs = new List<ApplicationBranch>();
            foreach (var branch in branchs)
            {
                var modules = await ScanBranchAsync(repository, branch.Name, cancellationToken);
                if (!modules.Any())
                {
                    continue;
                }

                var lastCommitDate = DateTimeOffset.Parse(branch.Commit.CommittedDate, CultureInfo.CurrentCulture);

                applicationBranchs.Add(ApplicationBranch.From(branch.Name, repository.HttpUrlToRepo.Replace(".git", "/") + BranchLinkSuffix(branch.Name), modules.Where(m => m is not null).ToArray(), lastCommitDate.UtcDateTime));
            }

            if (applicationBranchs.Count == 0)
            {
                return null;
            }

            return Application.From(repository.Name, repository.Namespace.FullPath.Replace("/", ">"), repository.HttpUrlToRepo.Replace(".git", "/"), applicationBranchs, RepositoryType.Gitlab);
        }
        catch (TaskCanceledException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (GitLabException ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of Gitlab Error : {ExceptionMessage}.", repository.Name, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", repository.Name, ex.GetType().Name, ex.Message);
        }

        return null;
    }

    private async Task<IReadOnlyCollection<File>> FindModuleFiles(int projectId, string branchName)
    {
        var tree = await GitLabClient.Trees.GetAsync(projectId, o => { o.Recursive = true; o.Reference = branchName; });
        var fileHeaders = tree.Where(f => ModuleParsers.Any(p => p.IsModuleFile(f.Path)));
        var filesTask = fileHeaders.Select(fh => GitLabClient.Files.GetAsync(projectId, fh.Path, branchName));
        return await Task.WhenAll(filesTask);
    }

    private async Task<IReadOnlyCollection<Branch>> FindAllLongTermBranchs(int projectId)
    {
        var branches = await GitLabClient.Branches.GetAsync(projectId, o => { });
        var branchsNames = branches.Select(b => b.Name).Intersect(Constants.Git.ValidBranches);
        return branches.Where(b => branchsNames.Contains(b.Name)).ToArray();
    }

    private async Task<IEnumerable<ApplicationModule>> ScanBranchAsync(Project project, string branchName, CancellationToken cancellationToken)
    {
        var moduleFiles = await FindModuleFiles(project.Id, branchName);
        if (moduleFiles.Count == 0)
        {
            return [];
        }

        var modules = new List<ApplicationModule>();
        foreach (var moduleFile in moduleFiles)
        {
            try
            {
                var module = await ScanModuleAsync(moduleFile, cancellationToken);
                if (module is null)
                {
                    continue;
                }

                modules.Add(module);
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Module {ModuleName} in Branch {BranchName}, Application {ApplicationName} skipped because of {ExceptionType} : {ExceptionMessage}.", moduleFile.Filename, branchName, project.Name, ex.GetType().Name, ex.Message);
            }
        }

        return modules;
    }

    private async Task<ApplicationModule?> ScanModuleAsync(File moduleFile, CancellationToken cancellationToken)
    {
        var moduleParser = ModuleParsers.FirstOrDefault(mp => mp.CanParse(moduleFile.ContentDecoded));
        if (moduleParser is null)
        {
            return null;
        }

        return await moduleParser.ParseModuleAsync(moduleFile.ContentDecoded, moduleFile.Filename, cancellationToken);
    }
}