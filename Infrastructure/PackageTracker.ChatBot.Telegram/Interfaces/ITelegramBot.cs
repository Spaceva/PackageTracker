using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.ChatBot.Telegram;
public interface ITelegramBot : IChatBot
{
    IReplyMarkup? BuildButtons(params string[] buttons);
    IReplyMarkup? BuildButtons(params Tuple<string, string>[] buttons);
    IReplyMarkup? BuildButtons(IEnumerable<KeyValuePair<string, string>> buttons);
    IReplyMarkup? BuildButtons(IDictionary<string,string> buttons);
    IReplyMarkup? BuildButtons<T>(Func<T, InlineKeyboardButton> mapperToButton, params T[] buttons);
    IReplyMarkup? BuildButtons<T>(IEnumerable<IEnumerable<T>> buttons, Func<T, InlineKeyboardButton> mapperToButton);
    IReplyMarkup? BuildButtons<TEnum>();
    Task<Dictionary<UserId, ChatPermission>> GetChatAdminsAsync(ChatId chatId, CancellationToken cancellationToken = default);
    Task<ChatPermission?> GetChatPermissionAsync(ChatId chatId, UserId userId, CancellationToken cancellationToken = default);
    Task<UserId> GetChatCreatorAsync(ChatId chatId, CancellationToken cancellationToken = default);
    Task SendLocationAsync(ChatId chatId, float lat, float lng, CancellationToken cancellationToken = default);
}