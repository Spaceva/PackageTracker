using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.Serialization;

namespace PackageTracker.Monitor.EndOfLife;
internal class DateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly IFormatProvider formatProvider = new DateTimeFormat("YYYY-MM-DD").FormatProvider;

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            return null;
        }

        var content = reader.GetString() ?? string.Empty;
        return new DateTime(DateOnly.Parse(content, formatProvider), TimeOnly.MinValue, DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Value.ToString("YYYY-MM-DD"));
    }
}