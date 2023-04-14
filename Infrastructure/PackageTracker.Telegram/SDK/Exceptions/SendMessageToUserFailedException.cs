using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class SendMessageToUserFailedException : Exception
{
    public UserId UserId { get; }
    public ITelegramSendingMessageOptions? SendingMessageOptions { get; }

    public SendMessageToUserFailedException(UserId userId, ITelegramSendingMessageOptions? messageOptions = null)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToUserFailedException(string message, UserId userId, ITelegramSendingMessageOptions? messageOptions = null) : base(message)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToUserFailedException(string message, UserId userId, Exception innerException, ITelegramSendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }
}
