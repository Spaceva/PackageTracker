﻿using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.Serialization;

namespace PackageTracker.Monitor.GitHub.NodeJS;
internal class NullableDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly IFormatProvider formatProvider = new DateTimeFormat("YYYY-MM-DD").FormatProvider;

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var content = reader.GetString() ?? string.Empty;
        if (content.Length == 0)
        {
            return null;
        }

        return new DateTime(DateOnly.Parse(content, formatProvider), TimeOnly.MinValue, DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            return;
        }

        writer.WriteStringValue(value.Value.ToString("YYYY-MM-DD"));
    }
}