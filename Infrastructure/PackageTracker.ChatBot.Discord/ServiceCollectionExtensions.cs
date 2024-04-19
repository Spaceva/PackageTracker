using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PackageTracker.ChatBot.Discord;

public static class ServiceCollectionExtensions
{
    public static void AddDiscordChatBot<TChatBot>(this IServiceCollection services, Func<IServiceProvider, TChatBot>? factory = null)
        where TChatBot : class, IDiscordBot
    {
        services.AddChatBot(factory);
        services.AddSingleton<IDiscordBot>(sp => sp.GetRequiredService<TChatBot>());
    }

    public static IConfigurationBuilder AddDiscordConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("discord.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"discord.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
