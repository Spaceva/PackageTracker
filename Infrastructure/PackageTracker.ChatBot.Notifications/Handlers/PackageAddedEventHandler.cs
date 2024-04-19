using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;

internal class PackageAddedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<PackageAddedEventHandler> logger) : ChatBotNotificationHandler<PackageAddedEvent>(chatBots, logger)
{
    protected override string? Message(PackageAddedEvent notification, IChatBot chatBot)
    => $"New Package {chatBot.Bold(notification.Type)} : {chatBot.Bold(notification.Name)} - v{chatBot.Italic(notification.LatestVersionLabel ?? "")} {chatBot.Italic(notification.NoReleasedVersion ? "[PRE-RELEASE]" : "[RELEASE]")}.";
}
