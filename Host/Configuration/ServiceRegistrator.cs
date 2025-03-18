using Microsoft.OpenApi.Models;
using PackageTracker.ApplicationModuleParsers;
using PackageTracker.Fetcher;
using PackageTracker.Fetcher.PublicRegistries;
using PackageTracker.Handlers;
using PackageTracker.Scanner;
using PackageTracker.Monitor;
using PackageTracker.Monitor.GitHub;
using PackageTracker.Monitor.EndOfLife;
using PackageTracker.Export.Confluence;
using PackageTracker.Presentation.WebApi;
using PackageTracker.Presentation.MVCApp;
using PackageTracker.Presentation.ExceptionHandlers;
using PackageTracker.ChatBot.Discord.Notifications;
using PackageTracker.Scanner.AzureDevOps;
using PackageTracker.Scanner.GitHub;
using PackageTracker.Scanner.Gitlab;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Host.Configuration;
internal static class ServiceRegistrator
{
    public static void RegisterServices(this WebApplicationBuilder builder)
     => builder.Services.AddServices(builder.Configuration);

    private static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMainHandlers().AddApplicationModuleParsers();

        services.AddModules(configuration);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Package Tracker API",
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

        services.ConfigureDatabase(configuration);

        services.AddExceptionHandlers();
    }

    public static async Task RegisterModulesAsync(this IModuleManager moduleManager)
    {
        await moduleManager.TryRegisterAsync(Fetcher.Constants.ModuleName);
        await moduleManager.TryRegisterAsync(Scanner.Constants.ModuleName);
        await moduleManager.TryRegisterAsync(Monitor.Constants.ModuleName);
        await moduleManager.TryRegisterAsync(Export.Confluence.Constants.ModuleName);
    }

    private static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFetcher(configuration)
                .AddPublicRegistriesFetchers();

        services.AddApplicationsScanner(configuration)
                .AddAzureDevOps()
                .AddGitHub()
                .AddGitlab();

        services.AddMonitor(configuration)
                .AddGitHubMonitors()
                .AddEndOfLifeMonitors();

        services.AddConfluenceExport(configuration);

        try
        {
            services.NotifyWithDiscord(configuration);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Notify with Discord disabled : {ex.ParamName} - {ex.Message}");
        }
        try
        {
            services.NotifyWithTelegram(configuration);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Notify with Telegram disabled : {ex.ParamName} - {ex.Message}");
        }

    }
}
