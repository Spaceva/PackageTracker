namespace PackageTracker.ChatBot.Notifications;
using MessageSendingAction = Func<IChatBot, string, CancellationToken, Task>;

internal static class ChatBotActionResolver
{
    private static readonly Dictionary<string, ICollection<MessageSendingAction>> store = [];

    public static void Register<TChatBot>(MessageSendingAction messageSendingAction) where TChatBot : IChatBot
    {
        var key = typeof(TChatBot).Name;
        if (store.TryGetValue(key, out ICollection<MessageSendingAction>? value))
        {
            value.Add(messageSendingAction);
            return;
        }

        store.Add(key, [messageSendingAction]);
    }

    public static IEnumerable<MessageSendingAction> GetActions(IChatBot chatBot)
    {
        var key = chatBot.GetType().Name;
        if (store.TryGetValue(key, out ICollection<MessageSendingAction>? messageSendingActions))
        {
            return messageSendingActions;
        }

        return [];
    }
}
