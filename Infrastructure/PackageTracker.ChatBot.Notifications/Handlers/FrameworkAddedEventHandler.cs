using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class FrameworkAddedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<FrameworkAddedEventHandler> logger) : ChatBotNotificationHandler<FrameworkAddedEvent>(chatBots, logger)
{
    protected override string Message(FrameworkAddedEvent notification, IChatBot chatBot)
     => $"Added {chatBot.Bold(notification.Framework.Name)} {notification.Framework.Version} {chatBot.Italic($"(Channel {notification.Framework.Channel})")}.";
}
