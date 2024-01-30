using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.Github.DotNet;

internal class MainIndexFile
{

    [JsonPropertyName("releases-index")]
    public IReadOnlyCollection<MainReleasesIndex> ReleasesIndex { get; init; } = Array.Empty<MainReleasesIndex>();
}