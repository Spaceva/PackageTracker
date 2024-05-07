namespace PackageTracker.Fetcher.PublicRegistries.PublicFetchers;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure.Http;
using System.Text.Json;

internal abstract class PublicPackageServerFetcher(string baseUrl, IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger logger) : IPackagesFetcher
{
    protected readonly HttpClient HttpClient = HttpClientFactory.Build(fetcherSettings.CurrentValue, baseUrl);

    protected IOptionsMonitor<FetcherSettings> FetcherSettings => fetcherSettings;

    public abstract string RegistryUrl { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<IReadOnlyCollection<Package>> FetchAsync(CancellationToken cancellationToken)
    {
        var packagesNames = PackagesName(FetcherSettings.CurrentValue);
        return await FetchPackagesAsync(packagesNames, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Package>> FetchAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken)
     => await FetchPackagesAsync(packagesName, cancellationToken);

    protected virtual void Dispose(bool disposing)
    {
        HttpClient?.Dispose();
    }

    private async Task<IReadOnlyCollection<Package>> FetchPackagesAsync(IEnumerable<string> packagesNames, CancellationToken cancellationToken)
    {
        var packages = new List<Package>();
        foreach (var packageName in packagesNames)
        {
            var package = await TryFetchPackageAsync(packageName, cancellationToken);
            if (package is null)
            {
                continue;
            }

            packages.Add(package);
        }

        return packages;
    }

    private async Task<Package?> TryFetchPackageAsync(string packageName, CancellationToken cancellationToken)
    {
        try
        {
            var response = await HttpClient.GetAsync(PackageRelativeUri(packageName), cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("GET '{PackageName}' Metadata failed: HTTP {StatusCodeInt} ({StatusCode})", packageName, (int)response.StatusCode, response.StatusCode);
                throw new HttpRequestException($"Get Metadata failed with HTTP {(int)response.StatusCode} ({response.StatusCode})");
            }

            using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var jsonDocument = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);
            var versions = Parse(jsonDocument!);
            if (versions.Count == 0)
            {
                return null;
            }

            return CreatePackage(packageName, versions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Skipped {Package} because of {ExceptionType}", packageName, ex.GetType().Name);
            return null;
        }
    }

    protected abstract Package CreatePackage(string packageName, ICollection<PackageVersion> packageVersions);

    protected abstract IEnumerable<string> PackagesName(FetcherSettings fetcherSettings);

    protected abstract ICollection<PackageVersion> Parse(JsonDocument jsonDocument);

    protected abstract string PackageRelativeUri(string packageName);
}