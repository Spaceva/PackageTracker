using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ChatBot;

public abstract class ChatBot(IServiceProvider serviceProvider) : IChatBot
{
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

    private ILogger? logger;

    protected ILogger Logger
    {
        get
        {
            logger ??= serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

            return logger;
        }
    }

    public async Task SendTextMessageToChatAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null)
    {
        if (incomingMessage is not null && incomingMessage.ChatId is not null)
        {
            await SendTextMessageToChatAsync(incomingMessage.ChatId, message, messageOptions);
        }
    }

    public async Task SendTextMessageToAuthorAsync(IIncomingMessage incomingMessage, string message, ISendingMessageOptions? messageOptions = null)
    {
        if (incomingMessage is not null && incomingMessage.AuthorUserId is not null)
        {
            await SendTextMessageToUserAsync(incomingMessage.AuthorUserId, message, messageOptions);
        }
    }

    public async Task SendTextMessageToUserAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        try
        {
            await SendTextMessageToUserInternalAsync(userId, message, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleSendingMessageExceptionAsync(new SendMessageToUserFailedException(ex.Message, userId, ex, messageOptions));
        }
    }

    public async Task SendTextMessageToChatAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        try
        {
            await SendTextMessageToChatInternalAsync(chatId, message, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleSendingMessageExceptionAsync(new SendMessageToChatFailedException(ex.Message, chatId, ex, messageOptions));
        }
    }

    public async Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null)
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

            await SendFileToUserInternalAsync(userId, dataStream, fileName, message, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleSendingFileExceptionAsync(new SendFileToUserFailedException(ex.Message, userId, dataStream, fileName, ex, messageOptions));
        }
    }

    public async Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null)
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

            await SendFileToChatInternalAsync(chatId, dataStream, fileName, message, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleSendingFileExceptionAsync(new SendFileToChatFailedException(ex.Message, chatId, dataStream, fileName, ex, messageOptions));
        }
    }

    public async Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToUserAsync(userId, dataStream, fileName, message, messageOptions);
    }

    public async Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToChatAsync(chatId, dataStream, fileName, message, messageOptions);
    }

    public async Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null)
    {
        var content = File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToUserAsync(userId, content, fileName, message, messageOptions);
    }

    public async Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ISendingMessageOptions? messageOptions = null)
    {
        var content = File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToChatAsync(chatId, content, fileName, message, messageOptions);
    }

    public async Task EditMessageAsync(IIncomingMessage incomingMessage, string newMessageContent, ISendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
        {
            return;
        }

        try
        {
            await EditMessageInternalAsync(incomingMessage.ChatId!, incomingMessage.MessageId!, newMessageContent, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, incomingMessage.MessageId!, incomingMessage.ChatId!, ex, messageOptions));
        }
    }

    public async Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
        {
            return;
        }

        try
        {
            await EditMessageInternalAsync(chatId, messageId, newMessageContent, messageOptions);
        }
        catch (Exception ex)
        {
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, messageId, chatId, ex, messageOptions));
        }
    }

    public async Task ReactToMessageAsync(IIncomingMessage incomingMessage, IEmoji emoji)
    {
        try
        {
            await ReactToMessageInternalAsync(incomingMessage.ChatId!, incomingMessage.MessageId!, emoji);
        }
        catch (Exception ex)
        {
            await HandleReactionMessageExceptionAsync(new ReactionMessageFailedException(ex.Message, incomingMessage.MessageId!, incomingMessage.ChatId!, ex, emoji));
        }
    }

    public async Task ReactToMessageAsync(ChatId chatId, MessageId messageId, IEmoji emoji)
    {
        try
        {
            await ReactToMessageInternalAsync(chatId, messageId, emoji);
        }
        catch (Exception ex)
        {
            await HandleReactionMessageExceptionAsync(new ReactionMessageFailedException(ex.Message, messageId, chatId, ex, emoji));
        }
    }

    public async Task SimulateTypingAsync(ChatId chatId)
    {
        Logger.LogDebug("Start SimulateTypingAsync on {chatId}", chatId);
        await SimulateTypingInternalAsync(chatId);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
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

    protected async Task HandleMessageUpdateAsync(IIncomingMessage incomingMessage)
    {
        if (incomingMessage.MessageText is null || string.IsNullOrEmpty(incomingMessage.MessageText))
        {
            Logger.LogDebug("Received event {MessageId} from {AuthorUserId}.", incomingMessage.MessageId, incomingMessage.AuthorUserId);
            await HandleEventUpdateAsync(incomingMessage);
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
            await HandleMessageTextUpdateAsync(incomingMessage);
            return;
        }

        var splitSpace = incomingMessage.MessageText.Split(' ');
        if (splitSpace.Length >= 1 && (splitSpace.First().Contains("@" + BotName, StringComparison.InvariantCultureIgnoreCase) || !splitSpace[0].Contains('@')))
        {
            var command = splitSpace[0].Replace("@" + BotName, "")[CommandStarterChar.Length..];
            var commandArgs = splitSpace.Length > 1 ? splitSpace.Skip(1).ToArray() : Array.Empty<string>();
            if (commandArgs.Length > 0)
            {
                var args = string.Join(";", commandArgs);
                Logger.LogDebug("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with args '{Args}'.", incomingMessage.MessageId, incomingMessage.ChatId, incomingMessage.AuthorUserId, command, args);
            }
            else
            {
                Logger.LogDebug("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with no args.", incomingMessage.MessageId, incomingMessage.ChatId, incomingMessage.AuthorUserId, command);
            }

            await HandleCommandUpdateAsync(incomingMessage, command, commandArgs);
        }
        else
        {
            Logger.LogDebug("Received message {MessageId} from {AuthorUserId}.", incomingMessage.MessageId, incomingMessage.AuthorUserId);
            await HandleMessageTextUpdateAsync(incomingMessage);
        }
    }

    protected abstract Task ReactToMessageInternalAsync(ChatId chatId, MessageId messageId, IEmoji emoji);

    protected abstract Task SimulateTypingInternalAsync(ChatId chatId);

    protected abstract Task HandleUpdateAsync(IIncomingMessage incomingMessage);

    protected abstract Task HandleMessageTextUpdateAsync(IIncomingMessage incomingMessage);

    protected abstract Task HandleCommandUpdateAsync(IIncomingMessage incomingMessage, string command, string[] commandArgs);

    protected abstract Task HandleEventUpdateAsync(IIncomingMessage incomingMessage);

    protected abstract Task HandleUpdateFailedAsync(IIncomingMessage incomingMessage, Exception ex);

    protected abstract Task HandleEditMessageExceptionAsync(EditMessageFailedException ex);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex);

    protected abstract Task HandleReactionMessageExceptionAsync(ReactionMessageFailedException ex);

    internal abstract Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, ISendingMessageOptions? messageOptions = null);

    internal abstract Task SendTextMessageToUserInternalAsync(UserId userId, string message, ISendingMessageOptions? messageOptions = null);

    internal abstract Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);

    internal abstract Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ISendingMessageOptions? messageOptions = null);

    internal abstract Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, ISendingMessageOptions? messageOptions = null);
}
