using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PackageTracker.ChatBot.Telegram;

public static class ServiceCollectionExtensions
{
    public static void AddTelegramChatBot<TChatBot>(this IServiceCollection services, Func<IServiceProvider, TChatBot>? factory = null)
        where TChatBot : class, ITelegramBot
    {
        services.AddChatBot(factory);
        services.AddSingleton<ITelegramBot>(sp => sp.GetRequiredService<TChatBot>());
    }

    public static IConfigurationBuilder AddTelegramConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("telegram.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"telegram.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
