using System.Text.Json.Serialization;

namespace PackageTracker.Monitor.EndOfLife;
internal class EndOfLifeHttpResponseElement
{
    public string Cycle { get; set; } = default!;

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? ReleaseDate { get; set; }

    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? Support { get; set; }

    [JsonPropertyName("eol")]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? EndOfLife { get; set; }
}
