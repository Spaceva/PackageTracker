﻿using Microsoft.OpenApi.Models;
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
using PackageTracker.Presentation.ExceptionHandlers;

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

    private static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
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
}