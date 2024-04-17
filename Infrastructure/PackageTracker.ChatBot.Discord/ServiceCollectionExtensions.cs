using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PackageTracker.ChatBot.Discord;

public static class ServiceCollectionExtensions
{
    public static void AddDiscordChatBot<TChatBot>(this IServiceCollection services)
        where TChatBot : class, IDiscordBot
    {
        services.AddChatBot<TChatBot>();
        services.AddSingleton<IDiscordBot>(sp => sp.GetRequiredService<TChatBot>());
    }

    public static void NotifyWithDiscord(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Discord");
        var token = section["Token"];
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        services.AddChatBot(sp => new DiscordNotifierBot(token, sp.GetRequiredService<IMediator>(), sp));
        services.AddSingleton<IDiscordBot>(sp => sp.GetRequiredService<DiscordNotifierBot>());
    }

    public static IConfigurationBuilder AddDiscordConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("discord.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"discord.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
