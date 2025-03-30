using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.GitHub.DotNet;

internal class MainReleasesIndex
{
    [JsonPropertyName("channel-version")]
    public string ChannelVersion { get; init; } = default!;

    [JsonPropertyName("latest-release")]
    public string LatestRelease { get; init; } = default!;

    [JsonPropertyName("latest-release-date")]
    public string LatestReleaseDate { get; init; } = default!;

    public string Product { get; init; } = default!;

    [JsonPropertyName("release-type")]
    public string ReleaseType { get; init; } = default!;

    [JsonPropertyName("support-phase")]
    public string SupportPhase { get; init; } = default!;

    [JsonPropertyName("eol-date")]
    [JsonConverter(typeof(NullableDateTimeConverter))]
    public DateTime? EolDate { get; init; }

    [JsonPropertyName("releases.json")]
    public string ReleasesJson { get; init; } = default!;
}
