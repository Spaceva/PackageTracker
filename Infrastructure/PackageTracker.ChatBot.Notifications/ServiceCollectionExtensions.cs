using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.ChatBot.Notifications;
using PackageTracker.ChatBot.Notifications.Configuration;
using PackageTracker.ChatBot.Notifications.Discord;
using System.Reflection;

namespace PackageTracker.ChatBot.Discord.Notifications;
public static class ServiceCollectionExtensions
{
    private static bool hasRegisteredHandlers;

    public static void NotifyWithDiscord(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Discord");
        var token = section["Token"];
        var notificationsSettings = section.GetSection("Notifications").Get<NotificationSettings[]>();
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        ArgumentNullException.ThrowIfNull(notificationsSettings);
        if (notificationsSettings.Length == 0)
        {
            throw new ArgumentException("Notifications settings can't be empty.");
        }

        services.AddDiscordChatBot(sp => new DiscordNotifierBot(token, sp.GetRequiredService<IMediator>(), sp));
        foreach (var notificationSettings in notificationsSettings)
        {
            RegisterNotificationAction<DiscordNotifierBot>(notificationSettings);
        }

        if (!hasRegisteredHandlers)
        {
            services.AddNotificationsHandler();
        }
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

        throw new ArgumentException("ChatType unknown", nameof(notificationSettings));
    }
}
