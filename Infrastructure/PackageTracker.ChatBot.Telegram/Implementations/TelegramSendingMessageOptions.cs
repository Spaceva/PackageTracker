using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.ChatBot.Telegram;

public class TelegramSendingMessageOptions : ISendingMessageOptions
{
    public LinkPreviewOptions LinkPreviewOptions { get; set; } = true;
    public bool DisableNotification { get; set; } = false;
    public ReplyParameters? ReplyParameters { get; set; } = null;
    public ReplyMarkup? ReplyMarkup { get; set; } = null;
}
