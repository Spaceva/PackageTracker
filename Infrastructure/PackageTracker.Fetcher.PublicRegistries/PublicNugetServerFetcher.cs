namespace PackageTracker.Fetcher.PublicRegistries;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Fetcher.PublicRegistries.PublicFetchers;
using System.Collections.Generic;
using System.Text.Json;
using static PackageTracker.Domain.Package.Constants;

internal class PublicNugetServerFetcher(IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger<PublicNugetServerFetcher> logger) : PublicPackageServerFetcher(Constants.PublicRegistryUrls.NUGET_API, fetcherSettings, logger)
{
    public override string RegistryUrl => Constants.PublicRegistryUrls.NUGET_PACKAGE;

    protected override IEnumerable<string> PackagesName(FetcherSettings fetcherSettings) => fetcherSettings.Packages?.Public?.Nuget ?? [];

    protected override ICollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => NugetPackageVersions(jsonDocument.RootElement.GetProperty("versions"));

    protected override string PackageRelativeUri(string packageName) => $"v3-flatcontainer/{packageName.ToLower()}/index.json";

    internal static ICollection<PackageVersion> NugetPackageVersions(JsonElement jsonElement)
        => [.. jsonElement.Deserialize<string[]>()!
                      .Where(v => RegularExpressions.AnyVersionNumber.IsMatch(v))
                      .Select(v => new PackageVersion(v))];

    protected override Package CreatePackage(string packageName, ICollection<PackageVersion> packageVersions)
     => new NugetPackage() { Name = packageName, Versions = packageVersions, RegistryUrl = RegistryUrl, Link = $"{RegistryUrl}/packages/{packageName}" };
}