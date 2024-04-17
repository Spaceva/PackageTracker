namespace PackageTracker.ChatBot;

public interface IChatBot
{
    string BeginBoldTag { get; }
    string BeginItalicTag { get; }
    UserId BotId { get; }
    string BotName { get; }
    string CommandStarterChar { get; }
    string EndBoldTag { get; }
    string EndItalicTag { get; }
    string Token { get; }
    bool IsReady { get; }

    Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null);
    Task EditMessageAsync(IIncomingMessage incomingMessage, string newMessageContent, ISendingMessageOptions? messageOptions = null);
    Task RunAsync(CancellationToken cancellationToken);
    Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null);
    Task SendTextMessageToAuthorAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null);
    Task SendTextMessageToChatAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null);
    Task SendTextMessageToChatAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null);
    Task SendTextMessageToUserAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null);
    Task ReactToMessageAsync(ChatId chatId, MessageId messageId, IEmoji emoji);
    Task ReactToMessageAsync(IIncomingMessage incomingMessage, IEmoji emoji);
    Task SimulateTypingAsync(ChatId chatId);

    string Bold(string text) => $"{BeginBoldTag}{text}{EndBoldTag}";
    string Italic(string text) => $"{BeginItalicTag}{text}{EndItalicTag}";
}