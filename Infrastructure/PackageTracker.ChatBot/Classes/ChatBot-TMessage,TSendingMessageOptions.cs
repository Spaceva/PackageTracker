namespace PackageTracker.ChatBot;

public abstract class ChatBot<TMessage, TSendingMessageOptions>(IServiceProvider serviceProvider) : ChatBot(serviceProvider)
where TMessage : class
where TSendingMessageOptions : class, ISendingMessageOptions
{
    internal override sealed async Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) 
        => await SendTextMessageToChatInternalAsync(chatId, message, (TSendingMessageOptions?)messageOptions, cancellationToken);

    internal override sealed async Task SendTextMessageToUserInternalAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) 
        => await SendTextMessageToUserInternalAsync(userId, message, (TSendingMessageOptions?)messageOptions, cancellationToken);

    internal override sealed async Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) 
        => await SendFileToChatInternalAsync(chatId, dataStream, fileName, message, (TSendingMessageOptions?)messageOptions, cancellationToken);

    internal override sealed async Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) 
        => await SendFileToUserInternalAsync(userId, dataStream, fileName, message, (TSendingMessageOptions?)messageOptions, cancellationToken);

    internal override sealed async Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) 
        => await EditMessageInternalAsync(chatId, messageId, newMessageContent, (TSendingMessageOptions?)messageOptions, cancellationToken);

    protected TMessage? GetOriginObject(IIncomingMessage incomingMessage)
    {
        if (incomingMessage is null)
        {
            return null;
        }

        return incomingMessage.OriginObject as TMessage;
    }

    protected abstract IIncomingMessage GetAbstractIncomingMessage(TMessage incomingMessage);

    protected abstract Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendTextMessageToUserInternalAsync(UserId userId, string message, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
}
