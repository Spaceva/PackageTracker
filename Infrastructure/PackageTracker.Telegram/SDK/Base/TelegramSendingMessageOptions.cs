using PackageTracker.Telegram.SDK.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.Telegram.SDK.Base;

internal class TelegramSendingMessageOptions : ITelegramSendingMessageOptions
{
    public bool DisableWebPagePreview { get; set; } = true;
    public bool DisableNotification { get; set; } = false;
    public int ReplyToMessageId { get; set; } = 0;
    public IReplyMarkup? ReplyMarkup { get; set; } = null;
}
