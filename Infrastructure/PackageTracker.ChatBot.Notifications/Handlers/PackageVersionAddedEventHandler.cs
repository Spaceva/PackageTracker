using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;

internal class PackageVersionAddedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<PackageVersionAddedEventHandler> logger) : ChatBotNotificationHandler<PackageVersionAddedEvent>(chatBots, logger)
{
    protected override string Message(PackageVersionAddedEvent notification, IChatBot chatBot)
     => $"New Package Version for {chatBot.Bold(notification.PackageName)}: v{chatBot.Bold(notification.PackageVersionLabel)}.";
}