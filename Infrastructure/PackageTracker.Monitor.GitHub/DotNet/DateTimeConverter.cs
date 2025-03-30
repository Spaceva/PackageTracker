using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.Serialization;

namespace PackageTracker.Monitor.GitHub.DotNet;
internal class DateTimeConverter : JsonConverter<DateTime>
{
    private static readonly IFormatProvider formatProvider = new DateTimeFormat("YYYY-MM-DD").FormatProvider;

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var content = reader.GetString() ?? throw new InvalidOperationException("Reader value did not get a non-empty string.");
        return new DateTime(DateOnly.Parse(content, formatProvider), TimeOnly.MinValue, DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("YYYY-MM-DD"));
    }
}