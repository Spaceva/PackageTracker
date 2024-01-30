using System.Text.Json;

namespace PackageTracker.Database.EntityFramework;
internal static class Constants
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() { IgnoreReadOnlyProperties = true, IgnoreReadOnlyFields = true, PropertyNameCaseInsensitive = true, WriteIndented = false };
}
