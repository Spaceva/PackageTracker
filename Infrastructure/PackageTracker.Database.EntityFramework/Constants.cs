using System.Text.Json;

namespace PackageTracker.Database.EntityFramework;
public static class Constants
{
    public const string SqlServer = nameof(SqlServer);
    public const string Postgres = nameof(Postgres);
    public const string InMemory = nameof(InMemory);
    
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new() { IgnoreReadOnlyProperties = true, IgnoreReadOnlyFields = true, PropertyNameCaseInsensitive = true, WriteIndented = false };
}
