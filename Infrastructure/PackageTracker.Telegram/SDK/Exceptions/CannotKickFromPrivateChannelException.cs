using PackageTracker.Telegram.SDK.Convertibles;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class CannotKickFromPrivateChannelException : Exception
{
    public ChatId ChatId { get; init; } = default!;

    public CannotKickFromPrivateChannelException(ChatId chatId)
    {
        ChatId = chatId;
    }
}
