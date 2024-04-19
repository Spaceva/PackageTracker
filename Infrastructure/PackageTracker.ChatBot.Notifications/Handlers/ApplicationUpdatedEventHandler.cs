using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ApplicationUpdatedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ApplicationUpdatedEventHandler> logger) : ChatBotNotificationHandler<ApplicationUpdatedEvent>(chatBots, logger)
{
    protected override string? Message(ApplicationUpdatedEvent notification, IChatBot chatBot)
     => $"Updated {notification.Path} > '{chatBot.Bold(notification.Name)}' ({chatBot.Bold(notification.ApplicationType)} application), source : {chatBot.Bold(notification.RepositoryType)}. IsSoonDecommissionned = {chatBot.Bold(notification.IsSoonDecommissionned)}, IsDeadLink = {chatBot.Bold(notification.IsDeadLink)}";
}
