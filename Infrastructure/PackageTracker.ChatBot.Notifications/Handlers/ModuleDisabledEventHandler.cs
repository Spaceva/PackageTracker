using Microsoft.Extensions.Logging;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal class ModuleDisabledEventHandler(IEnumerable<IChatBot> chatBots, ILogger<ModuleDisabledEventHandler> logger) : ChatBotNotificationHandler<ModuleDisabledEvent>(chatBots, logger)
{
    protected override string? Message(ModuleDisabledEvent notification, IChatBot chatBot)
     => $"Module {chatBot.Bold(notification.Name)} has been disabled.";
}
