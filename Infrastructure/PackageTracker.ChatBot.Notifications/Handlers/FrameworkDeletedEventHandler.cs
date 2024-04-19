using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class FrameworkDeletedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<FrameworkDeletedEventHandler> logger) : ChatBotNotificationHandler<FrameworkDeletedEvent>(chatBots, logger)
{
    protected override string? Message(FrameworkDeletedEvent notification, IChatBot chatBot)
     => $"Deleted {chatBot.Bold(notification.Name)} {chatBot.Italic(notification.Version)}.";
}
