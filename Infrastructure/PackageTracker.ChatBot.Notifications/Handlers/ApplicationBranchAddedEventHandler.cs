using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ApplicationBranchAddedEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ApplicationBranchAddedEventHandler> logger) : ChatBotNotificationHandler<ApplicationBranchAddedEvent>(chatBots, logger)
{
    protected override string? Message(ApplicationBranchAddedEvent notification, IChatBot chatBot)
    {
        var singleOrPlural = notification.Modules.Count > 1 ? "s" : string.Empty;
        return $"Added '{chatBot.Italic(notification.BranchName)}' branch to '{chatBot.Bold(notification.ApplicationName)}' ({chatBot.Bold(notification.Type)} application) - Counting {chatBot.Bold(notification.Modules.Count)} module{singleOrPlural}.";
    }
}
