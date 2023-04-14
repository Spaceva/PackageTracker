using PackageTracker.Domain.Packages.Model;
using System.Text.Json;
using static PackageTracker.Domain.Packages.Constants;

namespace PackageTracker.Fetcher;

internal static class JsonElementExtensions
{
    internal static IReadOnlyCollection<PackageVersion> GetNpmPackageVersions(this JsonElement jsonElement)
        => jsonElement.EnumerateObject()
                      .Where(property => RegularExpressions.AnyVersionNumber.IsMatch(property.Name))
                      .Select(version => new PackageVersion { Label = version.Name })
                      .OrderBy(version => version.Label)
                      .ToArray();

    internal static IReadOnlyCollection<PackageVersion> GetNugetPackageVersions(this JsonElement jsonElement)
        => jsonElement.Deserialize<string[]>()!
                      .Select(v => new PackageVersion { Label = v })
                      .ToArray();
}
