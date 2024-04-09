namespace PackageTracker.Host.Configuration;

internal class PersistenceSettings
{
    public const string ConfigurationSection = "Persistence";

    public string Type { get; init; } = default!;

    public bool UseMemoryCache { get; init; }
}
