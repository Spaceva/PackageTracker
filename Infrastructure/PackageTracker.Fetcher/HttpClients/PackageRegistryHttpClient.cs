using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Packages.Model;
using System.Text.Json;

namespace PackageTracker.Fetcher;
internal abstract class PackageRegistryHttpClient : HttpClient
{
    private readonly ILogger logger;

    public PackageRegistryHttpClient(ILogger logger)
    {
        this.logger = logger;
    }

    public async Task<IReadOnlyCollection<PackageVersion>> GetPackagesVersionsAsync(string packageName, CancellationToken stoppingToken = default)
    {
        var response = await this.GetAsync(PackageRelativeUri(packageName), stoppingToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("GET '{PackageName}' Metadata failed: HTTP {StatusCodeInt} ({StatusCode})", packageName, (int)response.StatusCode, response.StatusCode);
            throw new ApplicationException($"Get Metadata failed with HTTP {(int)response.StatusCode} ({response.StatusCode})");
        }

        using var responseBody = await response.Content.ReadAsStreamAsync(stoppingToken);
        using var jsonDocument = await JsonDocument.ParseAsync(responseBody, cancellationToken: stoppingToken);
        return Parse(jsonDocument!);
    }

    protected abstract IReadOnlyCollection<PackageVersion> Parse(JsonDocument jsonDocument);

    protected abstract string PackageRelativeUri(string packageName);
}
