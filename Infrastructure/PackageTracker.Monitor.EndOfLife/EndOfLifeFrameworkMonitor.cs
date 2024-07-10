using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Infrastructure.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace PackageTracker.Monitor.EndOfLife;
internal abstract class EndOfLifeFrameworkMonitor : IFrameworkMonitor
{
    private readonly string framework;

    protected EndOfLifeFrameworkMonitor(string framework, ILogger logger, IHttpProxy? httpProxy)
    {
        this.framework = framework;
        Logger = logger;
        HttpClient = HttpClientFactory.Build(httpProxy, Constants.Host, true);
        HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("PackageTracker", "v1.1.0"));
    }

    protected ILogger Logger { get; }

    protected HttpClient HttpClient { get; }

    public async Task<IReadOnlyCollection<Framework>> MonitorAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var httpResponseMessage = await HttpClient.GetAsync($"{Constants.Api}/{framework}.json", cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();
            var elements = await httpResponseMessage.Content.ReadFromJsonAsync<IReadOnlyCollection<EndOfLifeHttpResponseElement>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken) ?? throw new JsonException("End of life Parsing error");
            return await ParseAsync(elements, cancellationToken);
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

    protected abstract Task<IReadOnlyCollection<Framework>> ParseAsync(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements, CancellationToken cancellationToken);

    protected virtual void Dispose(bool isDisposing)
    {
        HttpClient?.Dispose();
    }
}
