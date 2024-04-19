namespace PackageTracker.ChatBot;

public abstract class ChatBot<TMessage, TIncomingMessage, TSendingMessageOptions, TChatBotCommand>(IServiceProvider serviceProvider) : ChatBot(serviceProvider)
where TMessage : class
where TIncomingMessage : class, IIncomingMessage
where TSendingMessageOptions : class, ISendingMessageOptions
where TChatBotCommand : class, IChatBotCommand
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

    internal override sealed async Task HandleCommandUpdateAsync(IIncomingMessage incomingMessage, string command, string[] commandArgs, CancellationToken cancellationToken = default)
        => await Mediator.Send(ParseCommand(command, commandArgs, (TIncomingMessage)incomingMessage), cancellationToken);

    internal override sealed async Task HandleMessageTextUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
        => await HandleMessageTextUpdateAsync((TIncomingMessage)incomingMessage, cancellationToken);

    internal override sealed async Task HandleUpdateFailedAsync(IIncomingMessage incomingMessage, Exception ex, CancellationToken cancellationToken = default)
        => await HandleUpdateFailedAsync((TIncomingMessage)incomingMessage, ex, cancellationToken);

    internal sealed override async Task HandleEventUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
        => await HandleEventUpdateInternalAsync((TIncomingMessage)incomingMessage, cancellationToken);

    protected TMessage? GetOriginObject(TIncomingMessage incomingMessage)
    {
        if (incomingMessage is null)
        {
            return null;
        }

        return incomingMessage.OriginObject as TMessage;
    }

    protected abstract TIncomingMessage ParseIncomingMessage(TMessage incomingMessage);

    protected abstract TChatBotCommand ParseCommand(string command, string[] commandArgs, TIncomingMessage incomingMessage);

    protected abstract Task HandleMessageTextUpdateAsync(TIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleUpdateFailedAsync(TIncomingMessage incomingMessage, Exception ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleEventUpdateInternalAsync(TIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendTextMessageToUserInternalAsync(UserId userId, string message, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    protected abstract Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, TSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
}
