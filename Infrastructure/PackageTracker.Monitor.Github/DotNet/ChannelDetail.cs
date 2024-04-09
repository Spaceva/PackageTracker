using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.Github.DotNet;


public class ChannelDetail
{
    [JsonPropertyName("channel-version")]
    public string ChannelVersion { get; init; } = default!;

    [JsonPropertyName("releases")]
    public IReadOnlyCollection<Release> Releases { get; init; } = [];

    public class Release
    {
        [JsonPropertyName("release-date")]
        public DateTime ReleaseDate { get; init; } = default!;

        [JsonPropertyName("release-version")]
        public string ReleaseVersion { get; init; } = default!;
    }
}
