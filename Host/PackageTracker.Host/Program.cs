using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PackageTracker.ApplicationModuleParsers;
using PackageTracker.Fetcher;
using PackageTracker.Fetcher.PublicRegistries;
using PackageTracker.Handlers;
using PackageTracker.Scanner;
using PackageTracker.Monitor;
using PackageTracker.Monitor.Github;
using PackageTracker.Monitor.EndOfLife;
using PackageTracker.Export.Confluence;
using PackageTracker.Presentation.WebApi;
using PackageTracker.Presentation.MVCApp;
using PackageTracker.Database.MongoDb;
using PackageTracker.Database.MemoryCache;
using PackageTracker.Host.Configuration;
using PackageTracker.Database.EntityFramework;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AddConfigurations(builder);

var application = builder.Build();

Configure(application);

application.Run();

static void Configure(WebApplication application)
{
    ConfigurePipeline(application, application.Environment);

    ConfigureEndpoints(application);

    StartDatabase(application);
}

static void AddConfigurations(WebApplicationBuilder builder)
{
    AddConfiguration(builder.Configuration, builder.Environment);
    AddServices(builder.Services, builder.Configuration);
    AddLoggingConfiguration(builder.Host);
}

static void AddLoggingConfiguration(ConfigureHostBuilder host)
    => host.UseSerilog(ConfigureSerilog);

static void ConfigureSerilog(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    Directory.CreateDirectory("logs");
    Serilog.Debugging.SelfLog.Enable(new StreamWriter("logs/serilog-failures.txt", true));
}

static void AddConfiguration(IConfigurationBuilder configuration, IHostEnvironment environment)
 => configuration.AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"logging.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                 .AddFetcherConfiguration(environment)
                 .AddScannerConfiguration(environment)
                 .AddMonitorConfiguration(environment)
                 .AddConfluenceConfiguration(environment);


static void AddModules(IServiceCollection services, IConfiguration configuration)
{
    services.AddMainHandlers().AddApplicationModuleParsers();

    var modules = configuration.GetSection("Modules");
    if (modules.GetValue<bool>("Fetcher"))
    {
        services.AddFetcher(configuration)
                .AddPublicRegistriesFetchers();
        // Add your fetcher registrations here
    }

    if (modules.GetValue<bool>("Scanner"))
    {
        services.AddScanner(configuration);
        // Add your scanner registrations here
    }

    if (modules.GetValue<bool>("Monitor"))
    {
        services.AddMonitor(configuration)
                .AddGithubMonitors()
                .AddEndOfLifeMonitors();
        // Add your monitor registrations here
    }

    if (modules.GetValue<bool>("ConfluenceExport"))
    {
        services.AddConfluenceExport(configuration);
        // Add your confluence exports registrations here
    }
}

static void AddServices(IServiceCollection services, IConfiguration configuration)
{
    AddModules(services, configuration);

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "PackageTrackers API",
            Description = "An ASP.NET Web API for managing obsolescence",
        });
    });

    services.AddAutoMapper(cfg =>
    {
        cfg.ConfigureMVCAppMappers();
        cfg.ConfigureApiMappers();
    });

    services.AddWebApiServices();

    services.AddMvcAppServices();

    services.AddHealthChecks();

    services.AddOutputCache();

    ConfigureDatabase(services, configuration);

    services.AddExceptionHandler((opt) =>
    {
        opt.ExceptionHandler = (context) =>
        {
            context.RequestServices.GetRequiredService<ILogger<RequestDelegate>>().LogError("Unhandled exception occured at {Method} {Route}.", context.Request.Method, context.Request.GetDisplayUrl());
            return Task.CompletedTask;
        };
    });
}

static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    var section = configuration.GetRequiredSection(PersistenceSettings.ConfigurationSection);
    services.Configure<PersistenceSettings>(section);
    var persistenceSettings = section.Get<PersistenceSettings>();
    ArgumentNullException.ThrowIfNull(persistenceSettings);

    if (persistenceSettings.Type.Equals(PackageTracker.Database.EntityFramework.Constants.PersistenceType))
    {
        services.AddEFDatabase(configuration);
    }
    else if (persistenceSettings.Type.Equals(PackageTracker.Database.MongoDb.Constants.PersistenceType))
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

static void ConfigurePipeline(IApplicationBuilder application, IWebHostEnvironment environment)
{
    application.ConfigureMVCAppPipeline(environment);

    application.UseSwagger();
    application.UseSwaggerUI();
    application.UseOutputCache();
    application.UseHttpsRedirection();
    application.UseExceptionHandler();
}

static void ConfigureEndpoints(IEndpointRouteBuilder application)
{
    application.ConfigureControllerEndpoints();
    application.MapGroup("/api").ConfigureApiEndpoints();
    application.MapHealthChecks("/health").ShortCircuit();
}

static void StartDatabase(IApplicationBuilder application)
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
    if (settingsValue.Type.Equals(PackageTracker.Database.EntityFramework.Constants.PersistenceType))
    {
        application.ApplicationServices.EnsureDatabaseIsUpdatedAsync().Wait();
    }
}