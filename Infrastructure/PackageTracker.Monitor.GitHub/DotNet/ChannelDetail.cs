﻿using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.GitHub.DotNet;


public class ChannelDetail
{
    [JsonPropertyName("channel-version")]
    public string ChannelVersion { get; init; } = default!;

    [JsonPropertyName("releases")]
    public IReadOnlyCollection<Release> Releases { get; init; } = [];

    public class Release
    {
        [JsonPropertyName("release-date")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime ReleaseDate { get; init; } = default!;

        [JsonPropertyName("release-version")]
        public string ReleaseVersion { get; init; } = default!;
    }
}
