using PackageTracker.Telegram.SDK.Convertibles;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class CannotLeavePrivateChannelException : Exception
{
    public ChatId ChatId { get; init; } = default!;

    public CannotLeavePrivateChannelException(ChatId chatId)
    {
        ChatId = chatId;
    }
}
