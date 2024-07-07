using PackageTracker.Fetcher;
using PackageTracker.Scanner;
using PackageTracker.Monitor;
using PackageTracker.Export.Confluence;
using PackageTracker.ChatBot.Discord;
using PackageTracker.ChatBot.Telegram;

namespace PackageTracker.Host.Configuration;
internal static class ConfigurationRegistrator
{
    public static void RegisterConfigurations(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        configuration.AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"logging.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddFetcherConfiguration(environment)
                        .AddApplicationScannerConfiguration(environment)
                        .AddMonitorConfiguration(environment)
                        .AddConfluenceConfiguration(environment)
                        .AddDiscordConfiguration(environment)
                        .AddTelegramConfiguration(environment);
    }
}
