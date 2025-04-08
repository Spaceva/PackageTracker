using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.ChatBot;

public abstract class ChatBot(IServiceProvider serviceProvider) : IChatBot
{
    private IMediator? mediator;

    private ILogger? logger;

    public abstract string BeginBoldTag { get; }

    public abstract string EndBoldTag { get; }

    public abstract string BeginItalicTag { get; }

    public abstract string EndItalicTag { get; }

    public abstract string CommandStarterChar { get; }

    public abstract string Token { get; }

    public virtual string BotName
    {
        get
        {
            return GetType().Name;
        }
    }

    public abstract UserId BotId { get; }

    public abstract bool IsReady { get; }

    protected virtual int TimeoutInSec => 3;

    protected ILogger Logger
    {
        get
        {
            logger ??= serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

            return logger;
        }
    }

    protected IMediator Mediator
    {
        get
        {
            mediator ??= serviceProvider.GetRequiredService<IMediator>();

            return mediator;
        }
    }

    public async Task SendTextMessageToChatAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (incomingMessage is not null && incomingMessage.ChatId is not null)
        {
            await SendTextMessageToChatAsync(incomingMessage.ChatId, message, messageOptions, cancellationToken);
        }
    }

    public async Task SendTextMessageToChatAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        try
        {
            await SendTextMessageToChatInternalAsync(chatId, message, messageOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send message to chat {ChatId}.", chatId);
            await HandleSendingMessageExceptionAsync(new SendMessageToChatFailedException(ex.Message, chatId, ex, messageOptions), cancellationToken);
        }
    }

    public async Task SendTextMessageToAuthorAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (incomingMessage is not null && incomingMessage.AuthorUserId is not null)
        {
            await SendTextMessageToUserAsync(incomingMessage.AuthorUserId, message, messageOptions, cancellationToken);
        }
    }

    public async Task SendTextMessageToUserAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        try
        {
            await SendTextMessageToUserInternalAsync(userId, message, messageOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send message to user {UserId}.", userId);
            await HandleSendingMessageExceptionAsync(new SendMessageToUserFailedException(ex.Message, userId, ex, messageOptions), cancellationToken);
        }
    }

    public async Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (dataStream is null || dataStream.Length == 0)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            await SendFileToUserInternalAsync(userId, dataStream, fileName, message, messageOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send file '{FileName}' to user {UserId}.", fileName, userId);
            await HandleSendingFileExceptionAsync(new SendFileToUserFailedException(ex.Message, userId, dataStream, fileName, ex, messageOptions), cancellationToken);
        }
    }

    public async Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToUserAsync(userId, dataStream, fileName, message, messageOptions, cancellationToken);
    }

    public async Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        var content = File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToUserAsync(userId, content, fileName, message, messageOptions, cancellationToken);
    }

    public async Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (dataStream is null || dataStream.Length == 0)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            await SendFileToChatInternalAsync(chatId, dataStream, fileName, message, messageOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send file '{FileName}' to chat {ChatId}.", fileName, chatId);
            await HandleSendingFileExceptionAsync(new SendFileToChatFailedException(ex.Message, chatId, dataStream, fileName, ex, messageOptions), cancellationToken);
        }
    }

    public async Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToChatAsync(chatId, dataStream, fileName, message, messageOptions, cancellationToken);
    }

    public async Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        var content = File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToChatAsync(chatId, content, fileName, message, messageOptions, cancellationToken);
    }

    public async Task EditMessageAsync(IIncomingMessage incomingMessage, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
        {
            return;
        }

        try
        {
            await EditMessageInternalAsync(incomingMessage.ChatId!, incomingMessage.MessageId!, newMessageContent, messageOptions, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to edit message #{MessageId} in chat {ChatId}.", incomingMessage.MessageId, incomingMessage.ChatId);
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, incomingMessage.MessageId!, incomingMessage.ChatId!, ex, messageOptions), cancellationToken);
        }
    }

    public async Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
        {
            return;
        }

        try
        {
            await EditMessageInternalAsync(chatId, messageId, newMessageContent, messageOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to edit message #{MessageId} in chat {ChatId}.", messageId, chatId);
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, messageId, chatId, ex, messageOptions), cancellationToken);
        }
    }

    public async Task ReactToMessageAsync(IIncomingMessage incomingMessage, IEmoji emoji, CancellationToken cancellationToken = default)
    {
        try
        {
            await ReactToMessageInternalAsync(incomingMessage.ChatId!, incomingMessage.MessageId!, emoji, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to react message #{MessageId} in chat {ChatId} with emoji {Emoji}.", incomingMessage.MessageId, incomingMessage.ChatId, emoji);
            await HandleReactionMessageExceptionAsync(new ReactionMessageFailedException(ex.Message, incomingMessage.MessageId!, incomingMessage.ChatId!, ex, emoji), cancellationToken);
        }
    }

    public async Task ReactToMessageAsync(ChatId chatId, MessageId messageId, IEmoji emoji, CancellationToken cancellationToken = default)
    {
        try
        {
            await ReactToMessageInternalAsync(chatId, messageId, emoji, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to react message #{MessageId} in chat {ChatId} with emoji {Emoji}.", messageId, chatId, emoji);
            await HandleReactionMessageExceptionAsync(new ReactionMessageFailedException(ex.Message, messageId, chatId, ex, emoji), cancellationToken);
        }
    }

    public async Task SimulateTypingAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Start SimulateTypingAsync on {ChatId}", chatId);
        await SimulateTypingInternalAsync(chatId, cancellationToken);
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await StartAsync(cancellationToken).ConfigureAwait(false);
            await Task.Delay(-1, cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            Logger.LogInformation("{BotType} has been stopped.", BotName);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("{BotType} has been stopped.", BotName);
        }

        await StopAsync(CancellationToken.None);
    }

    protected abstract Task StartAsync(CancellationToken cancellationToken);

    protected abstract Task StopAsync(CancellationToken cancellationToken);

    protected bool CheckIsBotTheSender(IIncomingMessage incomingMessage)
    {
        if (incomingMessage is null)
        {
            return false;
        }

        return BotId!.Equals(incomingMessage.AuthorUserId);
    }

    protected async Task SimulateTypingAsync(IIncomingMessage incomingMessage)
    {
        if (incomingMessage is not null)
        {
            await SimulateTypingAsync(incomingMessage.ChatId!);
        }
    }

    protected virtual async Task HandleUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await HandleMessageUpdateAsync(incomingMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to handle update #{MessageId} in chat {ChatId} from {AuthorUserId}.", incomingMessage.MessageId, incomingMessage.ChatId, incomingMessage.AuthorUserId);
            await HandleUpdateFailedAsync(incomingMessage, ex, cancellationToken);
        }
    }

    protected async Task HandleMessageUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
    {
        if (incomingMessage.MessageText is null || string.IsNullOrEmpty(incomingMessage.MessageText))
        {
            Logger.LogDebug("Received event {MessageId} from {AuthorUserId}.", incomingMessage.MessageId, incomingMessage.AuthorUserId);
            await HandleEventUpdateAsync(incomingMessage, cancellationToken);
            return;
        }

        if (incomingMessage.AuthorUserId is null
            || incomingMessage.MessageText is null
            || incomingMessage.AuthorUserId.Equals(BotId))
        {
            return;
        }

        if (!incomingMessage.MessageText.StartsWith(CommandStarterChar, StringComparison.InvariantCultureIgnoreCase))
        {
            Logger.LogDebug("Received message {MessageId} from {AuthorUserId}.", incomingMessage.MessageId!, incomingMessage.AuthorUserId!);
            await HandleMessageTextUpdateAsync(incomingMessage, cancellationToken);
            return;
        }

        var splitSpace = incomingMessage.MessageText.Split(' ');
        if (splitSpace.Length >= 1 && (splitSpace[0].Contains("@" + BotName, StringComparison.InvariantCultureIgnoreCase) || !splitSpace[0].Contains('@')))
        {
            var command = splitSpace[0].Replace("@" + BotName, "")[CommandStarterChar.Length..];
            var commandArgs = splitSpace.Length > 1 ? splitSpace.Skip(1).ToArray() : [];
            if (commandArgs.Length > 0)
            {
                var args = string.Join(";", commandArgs);
                Logger.LogDebug("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with args '{Args}'.", incomingMessage.MessageId, incomingMessage.ChatId, incomingMessage.AuthorUserId, command, args);
            }
            else
            {
                Logger.LogDebug("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with no args.", incomingMessage.MessageId, incomingMessage.ChatId, incomingMessage.AuthorUserId, command);
            }

            await HandleCommandUpdateAsync(incomingMessage, command, commandArgs, cancellationToken);
            return;
        }

        Logger.LogDebug("Received message {MessageId} from {AuthorUserId}.", incomingMessage.MessageId, incomingMessage.AuthorUserId);
        await HandleMessageTextUpdateAsync(incomingMessage, cancellationToken);
    }

    protected abstract Task ReactToMessageInternalAsync(ChatId chatId, MessageId messageId, IEmoji emoji, CancellationToken cancellationToken = default);

    protected abstract Task SimulateTypingInternalAsync(ChatId chatId, CancellationToken cancellationToken = default);

    protected abstract Task HandleEditMessageExceptionAsync(EditMessageFailedException ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex, CancellationToken cancellationToken = default);

    protected abstract Task HandleReactionMessageExceptionAsync(ReactionMessageFailedException ex, CancellationToken cancellationToken = default);

    internal abstract Task HandleMessageTextUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    internal abstract Task HandleCommandUpdateAsync(IIncomingMessage incomingMessage, string command, string[] commandArgs, CancellationToken cancellationToken = default);

    internal abstract Task HandleEventUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    internal abstract Task HandleUpdateFailedAsync(IIncomingMessage incomingMessage, Exception ex, CancellationToken cancellationToken = default);

    internal abstract Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    internal abstract Task SendTextMessageToUserInternalAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    internal abstract Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    internal abstract Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);

    internal abstract Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default);
}
