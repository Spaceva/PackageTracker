namespace PackageTracker.ChatBot;

[Serializable]
public class EditMessageFailedException : Exception
{
    public MessageId MessageId { get; private set; }
    public ChatId ChatId { get; private set; }
    public ISendingMessageOptions? SendingMessageOptions { get; private set; }

    public EditMessageFailedException(MessageId messageId, ChatId chatId, ISendingMessageOptions? messageOptions = null) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
    public EditMessageFailedException(string message, MessageId messageId, ChatId chatId, ISendingMessageOptions? messageOptions = null) : base(message) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
    public EditMessageFailedException(string message, MessageId messageId, ChatId chatId, Exception innerException, ISendingMessageOptions? messageOptions = null) : base(message, innerException) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
}
