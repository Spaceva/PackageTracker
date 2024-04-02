using System.Text.Json;

namespace PackageTracker.Database.EntityFramework;
public static class Constants
{
    public const string PersistenceType = "SqlServer";

    internal static readonly JsonSerializerOptions JsonSerializerOptions = new() { IgnoreReadOnlyProperties = true, IgnoreReadOnlyFields = true, PropertyNameCaseInsensitive = true, WriteIndented = false };
}
