using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Infrastructure.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static PackageTracker.Monitor.MonitorSettings;

namespace PackageTracker.Monitor.Github;
internal abstract class GithubFrameworkMonitor : IFrameworkMonitor
{
    private readonly string githubFileUrl;

    protected GithubFrameworkMonitor(MonitoredFramework monitoredFramework, ILogger logger, IHttpProxy? httpProxy)
    {
        githubFileUrl = monitoredFramework.Url;
        Logger = logger;
        HttpClient = HttpClientFactory.Build(httpProxy);
        HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("PackageTracker", "v0.0.1"));
    }

    protected ILogger Logger { get; }

    protected HttpClient HttpClient { get; }

    public async Task<IReadOnlyCollection<Framework>> MonitorAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpResponseMessage = await HttpClient.GetAsync(githubFileUrl, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();
            var gitHubFile = await httpResponseMessage.Content.ReadFromJsonAsync<GithubFile>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken) ?? throw new JsonException("GithubFile Parsing error");
            var decodedContent = gitHubFile.Encoding is not null && gitHubFile.Encoding.Equals("base64") ? Encoding.UTF8.GetString(Convert.FromBase64String(gitHubFile.Content)) : gitHubFile.Content;
            return await ParseGithubFileContentAsync(decodedContent, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning("Operation cancelled.");
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning("Error HTTP {StatusCode} : {ExceptionMessage}.", ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("{ExceptionType} : {ExceptionMessage}.", ex.GetType().Name, ex.Message);
        }
        return [];
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract Task<IReadOnlyCollection<Framework>> ParseGithubFileContentAsync(string decodedContent, CancellationToken cancellationToken);

    protected virtual void Dispose(bool isDisposing)
    {
        this.HttpClient.Dispose();
    }
}
