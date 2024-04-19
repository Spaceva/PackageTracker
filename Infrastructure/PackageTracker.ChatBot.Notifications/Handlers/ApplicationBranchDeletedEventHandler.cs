using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ApplicationBranchDeletedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ApplicationBranchDeletedEventHandler> logger) : ChatBotNotificationHandler<ApplicationBranchDeletedEvent>(chatBots, logger)
{
    protected override string? Message(ApplicationBranchDeletedEvent notification, IChatBot chatBot)
     => $"Deleted '{chatBot.Italic(notification.BranchName)}' branch from '{chatBot.Bold(notification.ApplicationName)}' ({chatBot.Bold(notification.Type)} application).";
}
