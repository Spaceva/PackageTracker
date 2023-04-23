using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Telegram.SDK.Base;
using PackageTracker.Telegram.SDK.Interfaces;
using System.Reflection;

namespace PackageTracker.Telegram;

public static class ServiceCollectionExtensions
{
    public static void AddTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        var currentSettings = configuration.GetSection("Telegram").Get<TelegramBotSettings>();

        if (IsNotValid(currentSettings))
        {
            return;
        }

        services.Configure<TelegramBotSettings>(configuration.GetSection("Telegram"));

        services.AddSingleton<ITelegramChatBot, PackageTrackerTelegramBot>();

        services.AddHostedService<TelegramBotHostingService>();

        services.AddMediatR(Assembly.GetExecutingAssembly());
    }

    private static bool IsNotValid(TelegramBotSettings? currentSettings)
    {
        return currentSettings is null || string.IsNullOrWhiteSpace(currentSettings.Token) || string.IsNullOrWhiteSpace(currentSettings.ChannelId);
    }
}
