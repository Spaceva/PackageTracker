using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.GitHub.NodeJS.Model;
internal class VersionDetail
{
    [JsonConverter(typeof(NullableDateTimeConverter))]
    public DateTime? Start { get; init; }

    [JsonConverter(typeof(NullableDateTimeConverter))]
    public DateTime? Lts { get; init; }

    [JsonConverter(typeof(NullableDateTimeConverter))]
    public DateTime? Maintenance { get; init; }

    [JsonConverter(typeof(NullableDateTimeConverter))]
    public DateTime? End { get; init; }
    public string? Codename { get; init; }
}
