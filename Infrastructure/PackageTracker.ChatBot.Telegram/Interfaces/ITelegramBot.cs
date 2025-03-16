using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.ChatBot.Telegram;
public interface ITelegramBot : IChatBot
{
    ReplyMarkup? BuildButtons(params string[] buttons);
    ReplyMarkup? BuildButtons(params Tuple<string, string>[] buttons);
    ReplyMarkup? BuildButtons(IEnumerable<KeyValuePair<string, string>> buttons);
    ReplyMarkup? BuildButtons(IDictionary<string, string> buttons);
    ReplyMarkup? BuildButtons<T>(Func<T, InlineKeyboardButton> mapperToButton, params T[] buttons);
    ReplyMarkup? BuildButtons<T>(IEnumerable<IEnumerable<T>> buttons, Func<T, InlineKeyboardButton> mapperToButton);
    ReplyMarkup? BuildButtons<TEnum>();
    Task<Dictionary<UserId, ChatPermission>> GetChatAdminsAsync(ChatId chatId, CancellationToken cancellationToken = default);
    Task<ChatPermission?> GetChatPermissionAsync(ChatId chatId, UserId userId, CancellationToken cancellationToken = default);
    Task<UserId> GetChatCreatorAsync(ChatId chatId, CancellationToken cancellationToken = default);
    Task SendLocationAsync(ChatId chatId, float lat, float lng, CancellationToken cancellationToken = default);
}