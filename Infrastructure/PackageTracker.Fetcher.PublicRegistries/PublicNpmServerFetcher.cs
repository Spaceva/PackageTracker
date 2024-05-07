namespace PackageTracker.Fetcher.PublicRegistries;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Fetcher.PublicRegistries.PublicFetchers;
using System.Collections.Generic;
using System.Text.Json;
using static PackageTracker.Domain.Package.Constants;

internal class PublicNpmServerFetcher(IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger<PublicNpmServerFetcher> logger) : PublicPackageServerFetcher(Constants.PublicRegistryUrls.NPM_API, fetcherSettings, logger)
{
    public override string RegistryUrl => Constants.PublicRegistryUrls.NPM_PACKAGE;

    protected override IEnumerable<string> PackagesName(FetcherSettings fetcherSettings) => fetcherSettings.Packages?.Public?.Npm ?? [];

    protected override ICollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => NpmPackageVersions(jsonDocument.RootElement.GetProperty("time"));

    protected override string PackageRelativeUri(string packageName) => packageName;

    internal static ICollection<PackageVersion> NpmPackageVersions(JsonElement jsonElement)
    => [.. jsonElement.EnumerateObject()
                  .Where(property => RegularExpressions.AnyVersionNumber.IsMatch(property.Name))
                  .Select(property => new PackageVersion(property.Name))
                  .OrderAscendingUsing(new PackageVersionComparer())];

    protected override Package CreatePackage(string packageName, ICollection<PackageVersion> packageVersions)
     => new NpmPackage() { Name = packageName, Versions = packageVersions, RegistryUrl = RegistryUrl, Link = $"{RegistryUrl}/package/{packageName}" };
}