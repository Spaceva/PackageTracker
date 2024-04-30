using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.GitHub.DotNet;

internal class MainIndexFile
{

    [JsonPropertyName("releases-index")]
    public IReadOnlyCollection<MainReleasesIndex> ReleasesIndex { get; init; } = [];
}