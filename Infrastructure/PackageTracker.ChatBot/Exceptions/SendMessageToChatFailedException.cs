namespace PackageTracker.ChatBot;

public class SendMessageToChatFailedException(string message, ChatId chatId, Exception innerException, ISendingMessageOptions? messageOptions = null) : Exception(message, innerException)
{
    public ChatId ChatId => chatId;
    public ISendingMessageOptions? SendingMessageOptions => messageOptions;
}
