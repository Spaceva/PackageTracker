using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.Telegram.SDK.Interfaces;

internal interface ITelegramSendingMessageOptions
{
    bool DisableWebPagePreview { get; }
    bool DisableNotification { get; }
    int ReplyToMessageId { get; }
    IReplyMarkup? ReplyMarkup { get; }
}
