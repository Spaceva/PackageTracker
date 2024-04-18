using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ApplicationAddedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ApplicationAddedEventHandler> logger) : ChatBotNotificationHandler<ApplicationAddedEvent>(chatBots, logger)
{
    protected override string Message(ApplicationAddedEvent notification, IChatBot chatBot)
     => $"Added {notification.Path} > '{chatBot.Bold(notification.Name)}' ({chatBot.Bold(notification.ApplicationType)} application). Source : {chatBot.Bold(notification.RepositoryType)}.";
}
