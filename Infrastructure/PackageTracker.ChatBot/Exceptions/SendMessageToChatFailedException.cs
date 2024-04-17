namespace PackageTracker.ChatBot;

[Serializable]
public class SendMessageToChatFailedException : Exception
{
    public ChatId ChatId { get; }
    public ISendingMessageOptions? SendingMessageOptions { get; }

    public SendMessageToChatFailedException(ChatId chatId, ISendingMessageOptions? messageOptions = null)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToChatFailedException(string message, ChatId chatId, ISendingMessageOptions? messageOptions = null) : base(message)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToChatFailedException(string message, ChatId chatId, Exception innerException, ISendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }
}
