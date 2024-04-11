using Microsoft.Extensions.Options;
using PackageTracker.Database.EntityFramework;
using PackageTracker.Database.MemoryCache;
using PackageTracker.Database.MongoDb;

namespace PackageTracker.Host.Configuration;
internal static class PersistenceConfigurator
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(PersistenceSettings.ConfigurationSection);
        services.Configure<PersistenceSettings>(section);
        var persistenceSettings = section.Get<PersistenceSettings>();
        ArgumentNullException.ThrowIfNull(persistenceSettings);

        if (persistenceSettings.Type.Equals(Database.EntityFramework.Constants.PersistenceType))
        {
            services.AddEFDatabase(configuration);
        }
        else if (persistenceSettings.Type.Equals(Database.MongoDb.Constants.PersistenceType))
        {
            services.AddMongoDatabase(configuration);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(configuration), "Unknown Persistence Type");
        }

        if (persistenceSettings.UseMemoryCache)
        {
            services.WithMemoryCache();
        }
    }

    public static void ConfigureDatabase(this IApplicationBuilder application)
    {
        var loggerFactory = application.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Database");

        var settings = application.ApplicationServices.GetRequiredService<IOptions<PersistenceSettings>>();
        if (settings is null || settings.Value is null)
        {
            logger.LogInformation("No database settings found.");
            return;
        }

        var settingsValue = settings.Value;
        logger.LogInformation("Database Type: {DatabaseType}, Use of Memory Cache: {UseMemoryCache}.", settingsValue.Type, settingsValue.UseMemoryCache ? "Yes" : "No");
        if (settingsValue.Type.Equals(Database.EntityFramework.Constants.PersistenceType))
        {
            application.ApplicationServices.EnsureDatabaseIsUpdatedAsync().Wait();
        }
    }
}
