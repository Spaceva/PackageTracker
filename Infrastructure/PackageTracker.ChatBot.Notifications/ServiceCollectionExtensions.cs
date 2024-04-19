using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.ChatBot.Notifications;
using PackageTracker.ChatBot.Notifications.Configuration;
using PackageTracker.ChatBot.Notifications.Discord;
using PackageTracker.ChatBot.Notifications.Telegram;
using PackageTracker.ChatBot.Telegram;
using System.Reflection;

namespace PackageTracker.ChatBot.Discord.Notifications;
public static class ServiceCollectionExtensions
{
    private static bool hasRegisteredHandlers;

    public static IServiceCollection NotifyWithDiscord(this IServiceCollection services, IConfiguration configuration)
    => services.NotifyWithChatBot<DiscordNotifierBot>(configuration,
        "Discord",
        (sc, token) => sc.AddDiscordChatBot(sp => new DiscordNotifierBot(token, sp)));

    public static IServiceCollection NotifyWithTelegram(this IServiceCollection services, IConfiguration configuration)
    => services.NotifyWithChatBot<TelegramNotifierBot>(configuration, 
        "Telegram",
        (sc, token) => sc.AddTelegramChatBot(sp => new TelegramNotifierBot(token, sp)));

    private static IServiceCollection NotifyWithChatBot<TChatBot>(this IServiceCollection services,
        IConfiguration configuration,
        string sectionName,
        Action<IServiceCollection, string> registerChatBot)
         where TChatBot : IChatBot
    {
        var section = configuration.GetSection(sectionName);
        var token = section["Token"];
        var notificationsSettings = section.GetSection("Notifications").Get<NotificationSettings[]>();
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(notificationsSettings);
        if (notificationsSettings.Length == 0)
        {
            throw new ArgumentException("Notifications settings can't be empty.");
        }

        registerChatBot(services, token);
        foreach (var notificationSettings in notificationsSettings)
        {
            RegisterNotificationAction<TChatBot>(notificationSettings);
        }

        if (!hasRegisteredHandlers)
        {
            services.AddNotificationsHandler();
        }

        return services;
    }

    private static IServiceCollection AddNotificationsHandler(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        hasRegisteredHandlers = true;
        return services;
    }

    private static void RegisterNotificationAction<TChatBot>(NotificationSettings notificationSettings) where TChatBot : IChatBot
    {
        if (notificationSettings.Type == NotificationSettings.ChatType.User)
        {
            ChatBotActionResolver.Register<TChatBot>((chatBot, message, cancellationToken) => chatBot.SendTextMessageToUserAsync(notificationSettings.ChatId, message, cancellationToken: cancellationToken));
            return;
        }

        if (notificationSettings.Type == NotificationSettings.ChatType.Channel)
        {
            ChatBotActionResolver.Register<TChatBot>((chatBot, message, cancellationToken) => chatBot.SendTextMessageToChatAsync(notificationSettings.ChatId, message, cancellationToken: cancellationToken));
            return;
        }

        throw new ArgumentException("ChatType unknown.", nameof(notificationSettings));
    }
}
