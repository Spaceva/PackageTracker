using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class EditMessageFailedException : Exception
{
    public MessageId MessageId { get; private set; }
    public ChatId ChatId { get; private set; }
    public ITelegramSendingMessageOptions? SendingMessageOptions { get; private set; }

    public EditMessageFailedException(MessageId messageId, ChatId chatId, ITelegramSendingMessageOptions? messageOptions = null) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
    public EditMessageFailedException(string message, MessageId messageId, ChatId chatId, ITelegramSendingMessageOptions? messageOptions = null) : base(message) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
    public EditMessageFailedException(string message, MessageId messageId, ChatId chatId, Exception innerException, ITelegramSendingMessageOptions? messageOptions = null) : base(message, innerException) { MessageId = messageId; ChatId = chatId; SendingMessageOptions = messageOptions; }
}
