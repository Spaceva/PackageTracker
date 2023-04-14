using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class SendMessageToChatFailedException : Exception
{
    public ChatId ChatId { get; }
    public ITelegramSendingMessageOptions? SendingMessageOptions { get; }

    public SendMessageToChatFailedException(ChatId chatId, ITelegramSendingMessageOptions? messageOptions = null)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToChatFailedException(string message, ChatId chatId, ITelegramSendingMessageOptions? messageOptions = null) : base(message)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToChatFailedException(string message, ChatId chatId, Exception innerException, ITelegramSendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
    }
}
