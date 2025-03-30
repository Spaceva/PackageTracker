using Microsoft.Extensions.Options;
using PackageTracker.Database.EntityFramework;
using PackageTracker.Database.MemoryCache;
using PackageTracker.Database.MongoDb;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Host.Configuration;
internal static class PersistenceConfigurator
{
    private static readonly string[] needingInitDbTypes = [Database.EntityFramework.Constants.Postgres, Database.EntityFramework.Constants.SqlServer];

    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetRequiredSection(PersistenceSettings.ConfigurationSection);
        services.Configure<PersistenceSettings>(section);
        var persistenceSettings = section.Get<PersistenceSettings>();
        ArgumentNullException.ThrowIfNull(persistenceSettings);

        if (persistenceSettings.Type.Equals(Database.EntityFramework.Constants.InMemory))
        {
            services.AddInMemoryEFDatabase();
            return;
        }

        if (persistenceSettings.UseMemoryCache)
        {
            services.WithMemoryCache();
        }

        if (persistenceSettings.Type.Equals(Database.EntityFramework.Constants.SqlServer))
        {
            services.AddSqlServerEFDatabase(configuration);
            return;
        }

        if (persistenceSettings.Type.Equals(Database.EntityFramework.Constants.Postgres))
        {
            services.AddPostgresEFDatabase(configuration);
            return;
        }

        if (persistenceSettings.Type.Equals(Database.MongoDb.Constants.PersistenceType))
        {
            services.AddMongoDatabase(configuration);
            return;
        }

        throw new ArgumentOutOfRangeException(nameof(configuration), "Unknown Persistence Type");
    }

    public static void ConfigureDatabase(this IApplicationBuilder application)
    {
        var loggerFactory = application.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Persistence");

        var settings = application.ApplicationServices.GetRequiredService<IOptions<PersistenceSettings>>();
        if (settings is null || settings.Value is null)
        {
            logger.LogInformation("No database settings found.");
            return;
        }

        var settingsValue = settings.Value;
        logger.LogInformation("Database Type: {DatabaseType}, Use of Memory Cache: {UseMemoryCache}.", settingsValue.Type, settingsValue.UseMemoryCache ? "Yes" : "No");
        if (needingInitDbTypes.Contains(settingsValue.Type))
        {
            application.ApplicationServices.EnsureDatabaseIsUpdatedAsync().Wait();
        }

        var moduleManager = application.ApplicationServices.GetRequiredService<IModuleManager>();
        moduleManager.RegisterModulesAsync().Wait();
    }
}
