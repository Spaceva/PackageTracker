using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ModuleEnabledEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ModuleEnabledEventHandler> logger) : ChatBotNotificationHandler<ModuleEnabledEvent>(chatBots, logger)
{
    protected override string? Message(ModuleEnabledEvent notification, IChatBot chatBot)
     => $"Module {chatBot.Bold(notification.Name)} has been enabled.";
}
