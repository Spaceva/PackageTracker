namespace PackageTracker.Fetcher;

using Microsoft.Extensions.Logging;
using static PackageTracker.Domain.Packages.Constants;
using PackageTracker.Domain.Packages.Model;
using System.Text.Json;

internal class NpmRegistryHttpClient : PackageRegistryHttpClient
{
    public NpmRegistryHttpClient(ILogger<NpmRegistryHttpClient> logger)
        : base(logger)
    {
        this.BaseAddress = new Uri(RegistryUrls.NPM_API);
    }

    protected override IReadOnlyCollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => jsonDocument.RootElement.GetProperty("time").GetNpmPackageVersions();

    protected override string PackageRelativeUri(string packageName) => packageName;
}
