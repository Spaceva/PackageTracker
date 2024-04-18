using MediatR;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ChatBot.Notifications.Handlers;
internal abstract class ChatBotNotificationHandler<TNotification>(IEnumerable<IChatBot> chatBots, ILogger logger) : INotificationHandler<TNotification>
    where TNotification : INotification
{
    public async Task Handle(TNotification notification, CancellationToken cancellationToken)
    {
        foreach (var chatBot in chatBots)
        {
            try
            {
                await Handle(notification, chatBot, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to notify {Notification} with {ChatBot}.", notification.GetType().Name, chatBot.BotName);
            }
        }
    }

    private async Task Handle(TNotification notification, IChatBot chatBot, CancellationToken cancellationToken)
    {
        var message = Message(notification, chatBot);
        await Task.WhenAll(ChatBotActionResolver.GetActions(chatBot).Select(a => a(chatBot, message, cancellationToken)));
    }

    protected abstract string Message(TNotification notification, IChatBot chatBot);
}
