namespace PackageTracker.ChatBot;

public class EditMessageFailedException(string message, MessageId messageId, ChatId chatId, Exception innerException, ISendingMessageOptions? messageOptions = null) : Exception(message, innerException)
{
    public MessageId MessageId => messageId;
    public ChatId ChatId => chatId;
    public ISendingMessageOptions? SendingMessageOptions => messageOptions;
}
