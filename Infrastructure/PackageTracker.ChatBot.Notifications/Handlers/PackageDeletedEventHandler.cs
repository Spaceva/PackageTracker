using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;

internal class PackageDeletedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<PackageDeletedEventHandler> logger) : ChatBotNotificationHandler<PackageDeletedEvent>(chatBots, logger)
{
    protected override string? Message(PackageDeletedEvent notification, IChatBot chatBot)
    => $"Removed Package {chatBot.Bold(notification.Type)} : {chatBot.Bold(notification.Name)} - {notification.Link}.";
}
