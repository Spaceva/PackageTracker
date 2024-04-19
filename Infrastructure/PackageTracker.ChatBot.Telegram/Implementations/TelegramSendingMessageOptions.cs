using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.ChatBot.Telegram;

public class TelegramSendingMessageOptions : ISendingMessageOptions
{
    public bool DisableWebPagePreview { get; init; } = true;
    public bool DisableNotification { get; init; } = false;
    public int ReplyToMessageId { get; init; } = 0;
    public IReplyMarkup? ReplyMarkup { get; init; } = null;
}
