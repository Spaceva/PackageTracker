namespace PackageTracker.Fetcher.PublicRegistries;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Fetcher.PublicRegistries.PublicFetchers;
using System.Collections.Generic;
using System.Text.Json;

internal class PublicMavenCentralServerFetcher : PublicPackageServerFetcher
{
    public PublicMavenCentralServerFetcher(IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger<PublicMavenCentralServerFetcher> logger) : base(Constants.PublicRegistryUrls.MAVENCENTRAL_API, fetcherSettings, logger)
    {
        HttpClient.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("PackageTracker", "v1.1.0"));
    }

    public override string RegistryUrl => Constants.PublicRegistryUrls.MAVENCENTRAL_PACKAGE;

    protected override IEnumerable<string> PackagesName(FetcherSettings fetcherSettings) => fetcherSettings.Packages?.Public?.MavenCentral ?? [];

    protected override ICollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => JavaPackageVersions(jsonDocument.RootElement.GetProperty("response").GetProperty("docs"));

    protected override string PackageRelativeUri(string packageName)
    {
        var splitPackageName = packageName.Split('.');
        var group = string.Join('.', splitPackageName[0..^1]);
        var name = splitPackageName[^1];
        return $"solrsearch/select?q=g:{group}+AND+a:{name}&core=gav&rows=200&wt=json";
    }

    internal static ICollection<PackageVersion> JavaPackageVersions(JsonElement jsonElement)
    => [.. jsonElement.EnumerateArray()
                  .Select(subJsonElement => subJsonElement.GetProperty("v"))
                  .Select(element => new PackageVersion(element.GetString()!))
                  .OrderAscendingUsing(new PackageVersionComparer())];

    protected override Package CreatePackage(string packageName, ICollection<PackageVersion> packageVersions)
    {
        var splitPackageName = packageName.Split('.');
        var group = string.Join('.', splitPackageName[0..^1]);
        var name = splitPackageName[^1];
        return new JavaPackage() { Name = packageName, Versions = packageVersions, RegistryUrl = RegistryUrl, Link = $"{RegistryUrl}/artifact/{group}/{name}" };
    }
}