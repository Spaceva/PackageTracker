using Microsoft.Extensions.Logging;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal abstract class ChatBotNotificationHandler<TNotification>(IEnumerable<IChatBot> chatBots, ILogger logger) : INotificationHandler<TNotification>
    where TNotification : INotification
{
    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        foreach (var chatBot in chatBots)
        {
            await HandleWithChatBot(notification, chatBot, cancellationToken);
        }
    }

    private async Task HandleWithChatBot(TNotification notification, IChatBot chatBot, CancellationToken cancellationToken)
    {
        try
        {
            var message = Message(notification, chatBot);
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            await Task.WhenAll(ChatBotActionResolver.GetActions(chatBot).Select(a => a(chatBot, message, cancellationToken)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify {Notification} with {ChatBot}.", notification.GetType().Name, chatBot.BotName);
        }
    }

    protected abstract string? Message(TNotification notification, IChatBot chatBot);
}
