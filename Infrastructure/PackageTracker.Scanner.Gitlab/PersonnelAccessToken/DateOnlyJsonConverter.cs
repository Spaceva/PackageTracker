using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PackageTracker.Scanner.Gitlab;

internal class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            DateOnly.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly dateValue,
        JsonSerializerOptions options) =>
            writer.WriteStringValue(dateValue.ToString(CultureInfo.InvariantCulture));
}