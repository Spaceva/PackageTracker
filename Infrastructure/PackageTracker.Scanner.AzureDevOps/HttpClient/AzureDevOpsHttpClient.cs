using PackageTracker.Infrastructure.Http;
using PackageTracker.Scanner.AzureDevOps.HttpClient.HttpResponses;
using PackageTracker.Scanner.AzureDevOps.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace PackageTracker.Scanner.AzureDevOps;

internal class AzureDevOpsHttpClient : IDisposable
{
    private const string PUBLICDOMAIN = "https://dev.azure.com";
    private readonly System.Net.Http.HttpClient httpClient;
    private readonly string domain = string.Empty;

    public AzureDevOpsHttpClient(string repositoryRootLink, string accessToken, IHttpProxy? httpProxy)
    {
        httpClient = HttpClientFactory.Build(httpProxy, repositoryRootLink);
        if (repositoryRootLink.StartsWith(PUBLICDOMAIN))
        {
            var uri = new Uri(repositoryRootLink);
            domain = uri.LocalPath + "/";
        }

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "Basic", accessToken))));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
        httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
    }

    public async Task<IReadOnlyCollection<Repository>> ListRepositoriesAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(domain + Constants.Urls.RepositoriesList, cancellationToken);
        response.EnsureJSONSuccess();
        var responseDecoded = await response.Content.ReadFromJsonAsync<RepositoriesListHttpResponse>(Constants.JsonSettings.JsonSerializerOptions, cancellationToken) ?? throw new SerializationFailedException();
        return responseDecoded.Value;
    }

    public async Task<IReadOnlyCollection<RepositoryBranch>> ListRepositoryBranchsAsync(string repositoryId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(string.Format(domain + Constants.Urls.RepositoryBranchsList, repositoryId), cancellationToken);
        response.EnsureJSONSuccess();

        var responseDecoded = await response.Content.ReadFromJsonAsync<RepositoryBranchsListHttpResponse>(Constants.JsonSettings.JsonSerializerOptions, cancellationToken) ?? throw new SerializationFailedException();
        return responseDecoded.Value;
    }

    public async Task<IReadOnlyCollection<Model.File>> GetFilesAsync(string repositoryId, string branch, CancellationToken cancellationToken)
    {
        var rootId = await GetRootIdAsync(repositoryId, cancellationToken);

        var response = await httpClient.GetAsync(string.Format(domain + Constants.Urls.FilesList, repositoryId, rootId, branch), cancellationToken);
        response.EnsureJSONSuccess();

        var responseDecoded = await response.Content.ReadFromJsonAsync<TreeListHttpResponse>(Constants.JsonSettings.JsonSerializerOptions, cancellationToken) ?? throw new SerializationFailedException();
        return responseDecoded.TreeEntries;
    }

    public async Task<string> GetFileContentAsync(string repositoryId, string branchId, string fileId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(string.Format(domain + Constants.Urls.FileDetail, repositoryId, fileId, branchId), cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<DateTime?> GetLastCommitAsync(string repositoryId, string branchName, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(string.Format(domain + Constants.Urls.LastCommit, repositoryId, branchName), cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseDecoded = await response.Content.ReadFromJsonAsync<GetLastCommitHttpResponse>(Constants.JsonSettings.JsonSerializerOptions, cancellationToken) ?? throw new SerializationFailedException();
        return responseDecoded.LastCommitDate;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async Task<string> GetRootIdAsync(string repositoryId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(string.Format(domain + Constants.Urls.RootDetail, repositoryId), cancellationToken);
        response.EnsureJSONSuccess();

        var responseDecoded = await response.Content.ReadFromJsonAsync<Folder>(Constants.JsonSettings.JsonSerializerOptions, cancellationToken) ?? throw new SerializationFailedException();
        return responseDecoded.ObjectId;
    }

    protected virtual void Dispose(bool isDisposing)
    {
        httpClient?.Dispose();
    }
}
