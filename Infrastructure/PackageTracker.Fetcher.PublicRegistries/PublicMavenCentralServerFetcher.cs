namespace PackageTracker.Fetcher.PublicRegistries;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Fetcher.PublicRegistries.PublicFetchers;
using System.Collections.Generic;
using System.Text.Json;

internal class PublicMavenCentralServerFetcher(IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger<PublicMavenCentralServerFetcher> logger) : PublicPackageServerFetcher(Constants.PublicRegistryUrls.MAVENCENTRAL_API, fetcherSettings, logger)
{
    public override string RegistryUrl => Constants.PublicRegistryUrls.MAVENCENTRAL_PACKAGE;

    protected override IEnumerable<string> PackagesName(FetcherSettings fetcherSettings) => fetcherSettings.Packages?.Public?.Packagist ?? [];

    protected override ICollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => PackagistPackageVersions(jsonDocument.RootElement.GetProperty("packages").EnumerateObject().Single().Value);

    protected override string PackageRelativeUri(string packageName) => $"{packageName}.json";

    internal static ICollection<PackageVersion> PackagistPackageVersions(JsonElement jsonElement)
    => [.. jsonElement.EnumerateArray()
                  .Select(subJsonElement => subJsonElement.GetProperty("version"))
                  .Select(element => new PackageVersion(element.GetString()!.Replace("v",string.Empty)))
                  .OrderAscendingUsing(new PackageVersionComparer())];

    protected override Package CreatePackage(string packageName, ICollection<PackageVersion> packageVersions)
     => new PackagistPackage() { Name = packageName, Versions = packageVersions, RegistryUrl = RegistryUrl, Link = $"{RegistryUrl}/{packageName}" };
}