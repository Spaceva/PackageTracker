namespace PackageTracker.Fetcher;

using Microsoft.Extensions.Logging;
using static Domain.Packages.Constants;
using PackageTracker.Domain.Packages.Model;
using System.Text.Json;

internal class NugetRegistryHttpClient : PackageRegistryHttpClient
{
    public NugetRegistryHttpClient(ILogger<NugetRegistryHttpClient> logger)
        : base(logger)
    {
        this.BaseAddress = new Uri(RegistryUrls.NUGET_API);
    }

    protected override IReadOnlyCollection<PackageVersion> Parse(JsonDocument jsonDocument)
     => jsonDocument.RootElement.GetProperty("versions").GetNugetPackageVersions();

    protected override string PackageRelativeUri(string packageName) => $"v3-flatcontainer/{packageName.ToLower()}/index.json";
}
