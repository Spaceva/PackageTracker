using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ApplicationDeletedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ApplicationDeletedEventHandler> logger) : ChatBotNotificationHandler<ApplicationDeletedEvent>(chatBots, logger)
{
    protected override string? Message(ApplicationDeletedEvent notification, IChatBot chatBot)
     => $"Deleted '{chatBot.Bold(notification.Name)}' ({chatBot.Bold(notification.Type)} application at {notification.RepositoryLink}).";
}
