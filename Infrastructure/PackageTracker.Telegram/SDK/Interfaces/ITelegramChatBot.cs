using PackageTracker.Telegram.SDK.Convertibles;
using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.Telegram.SDK.Interfaces;

internal interface ITelegramChatBot
{
    string BeginBoldTag { get; }
    string BeginItalicTag { get; }
    UserId BotId { get; }
    string BotName { get; }
    string EndBoldTag { get; }
    string EndItalicTag { get; }
    string Token { get; }

    Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ITelegramSendingMessageOptions? messageOptions = null);
    Task EditMessageAsync(ITelegramIncomingMessage update, string newMessageContent, ITelegramSendingMessageOptions? messageOptions = null);
    Task RunAsync(CancellationToken cancellationToken);
    Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendTextMessageToAuthorAsync(ITelegramIncomingMessage update, string message, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendTextMessageToChatAsync(ChatId chatId, string message, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendTextMessageToChatAsync(ITelegramIncomingMessage update, string message, ITelegramSendingMessageOptions? messageOptions = null);
    Task SendTextMessageToUserAsync(UserId userId, string message, ITelegramSendingMessageOptions? messageOptions = null);
    Task SimulateTypingAsync(ChatId chatId);

    IReplyMarkup? BuildButtons(params string[] buttons);
    IReplyMarkup? BuildButtons(params Tuple<string, string>[] buttons);
    IReplyMarkup? BuildButtons(IEnumerable<KeyValuePair<string, string>> buttons);
    IReplyMarkup? BuildButtons(IDictionary<string, string> buttons);
    IReplyMarkup? BuildButtons<T>(Func<T, InlineKeyboardButton> mapperToButton, params T[] buttons);
    IReplyMarkup? BuildButtons<T>(T[][] buttons, Func<T, InlineKeyboardButton> mapperToButton);
    IReplyMarkup? BuildButtons<TEnum>();
    Task<Dictionary<UserId, ChatPermission>> GetAdminsOfChatAsync(ChatId chatId);
    Task<ChatPermission?> GetChatPermissionAsync(ChatId chatId, UserId userId);
    Task<UserId> GetCreatorOfChatAsync(ChatId chatId);
    Task SendLocationAsync(ChatId chatId, float lat, float lng);
}