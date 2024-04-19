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

    Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task EditMessageAsync(IIncomingMessage incomingMessage, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task RunAsync(CancellationToken cancellationToken = default);
    Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendTextMessageToAuthorAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendTextMessageToChatAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendTextMessageToChatAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task SendTextMessageToUserAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
    Task ReactToMessageAsync(ChatId chatId, MessageId messageId, IEmoji emoji, CancellationToken cancellationToken = default);
    Task ReactToMessageAsync(IIncomingMessage incomingMessage, IEmoji emoji, CancellationToken cancellationToken = default);
    Task SimulateTypingAsync(ChatId chatId, CancellationToken cancellationToken = default);

    string Bold(string text) => $"{BeginBoldTag}{text}{EndBoldTag}";
    string Bold(object @object) => $"{BeginBoldTag}{@object}{EndBoldTag}";
    string Italic(string text) => $"{BeginItalicTag}{text}{EndItalicTag}";
    string Italic(object @object) => $"{BeginItalicTag}{@object}{EndItalicTag}";
}