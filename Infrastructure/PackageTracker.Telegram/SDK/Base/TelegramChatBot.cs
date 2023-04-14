using Microsoft.Extensions.Logging;
using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Exceptions;
using PackageTracker.Telegram.SDK.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using ChatId = PackageTracker.Telegram.SDK.Convertibles.ChatId;
using MessageId = PackageTracker.Telegram.SDK.Convertibles.MessageId;

namespace PackageTracker.Telegram.SDK.Base;

internal abstract class TelegramChatBot : ITelegramChatBot
{
    protected int offset = 0;

    private string? botName;

    private UserId? botID;

    public TelegramChatBot(ILogger logger)
    {
        Logger = logger;
    }

    public string BeginBoldTag => "<b>";
    public string EndBoldTag => "</b>";
    public string BeginItalicTag => "<i>";
    public string EndItalicTag => "</i>";
    public static string CommandStarterChar => "/";
    public string BotName
    {
        get
        {
            if (string.IsNullOrEmpty(botName))
            {
                botName = TelegramBotClient!.GetMeAsync().GetAwaiter().GetResult().Username!;
            }
            return botName;
        }
    }
    public UserId BotId
    {
        get
        {
            if (botID is null || string.IsNullOrEmpty(botID))
                botID = TelegramBotClient!.GetMeAsync().GetAwaiter().GetResult().Id!;
            return botID;
        }
    }
    public abstract string Token { get; }

    protected virtual int TimeoutInSec => 3;
    protected ILogger Logger { get; }
    protected ITelegramBotClient? TelegramBotClient { get; private set; }


    public async Task<UserId> GetCreatorOfChatAsync(ChatId chatId)
    {
        var admins = await GetAdminsOfChatAsync(chatId);
        return admins.FirstOrDefault(u => u.Value == (int)ChatMemberStatus.Creator).Key;
    }

    public async Task<Dictionary<UserId, ChatPermission>> GetAdminsOfChatAsync(ChatId chatId)
    {
        var admins = await TelegramBotClient!.GetChatAdministratorsAsync(chatId.ToString());
        return admins.ToDictionary(kvp => (UserId)kvp.User.Id.ToString(), kvp => (ChatPermission)(int)kvp.Status);
    }

    public async Task<ChatPermission?> GetChatPermissionAsync(ChatId chatId, UserId userId)
    {
        var rights = await TelegramBotClient!.GetChatMemberAsync(chatId.ToString(), userId);
        return (int?)rights?.Status;
    }

    public async Task SendLocationAsync(ChatId chatId, float lat, float lng)
    {
        Logger.LogDebug("Sending Location (lat:{lat},lng:{lng}) to Chat {chatId}.", lat, lng, chatId);
        await TelegramBotClient!.SendLocationAsync(chatId.ToString(), lat, lng);
    }

    public IReplyMarkup? BuildButtons(params Tuple<string, string>[] buttons) => BuildButtons(b => InlineKeyboardButton.WithCallbackData(b.Item1, b.Item2), buttons);

    public IReplyMarkup? BuildButtons(params string[] buttons) => BuildButtons(b => InlineKeyboardButton.WithCallbackData(b, b), buttons);

    public IReplyMarkup? BuildButtons(IEnumerable<KeyValuePair<string, string>> buttons) => BuildButtons(b => InlineKeyboardButton.WithCallbackData(b.Key, b.Value), buttons.ToArray());

    public IReplyMarkup? BuildButtons(IDictionary<string, string> buttons) => BuildButtons(buttons.AsEnumerable());

    public IReplyMarkup? BuildButtons<TEnum>() => BuildButtons((value) => InlineKeyboardButton.WithCallbackData(Enum.GetName(typeof(TEnum), value!)!, value!.ToString()!), Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToArray());

    public IReplyMarkup? BuildButtons<T>(Func<T, InlineKeyboardButton> mapperToButton, params T[] buttons)
    {
        var rows = new List<InlineKeyboardButton[]>();
        var buttonsCount = buttons.Length;
        if (buttonsCount == 0)
        {
            return null;
        }

        if (buttonsCount <= 4)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] { buttons.Select(b => mapperToButton(b)).ToArray() });
        }

        var rowContent = new List<InlineKeyboardButton>();
        int divisor = buttons.Length % 3 == 0 || buttons.Length % 3 == 2 ? 3 : 2;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > 0 && i % divisor == 0)
            {
                rows.Add(rowContent.ToArray());
                rowContent = new List<InlineKeyboardButton>();
            }
            rowContent.Add(mapperToButton(buttons[i]));
        }

        if (rowContent.Count > 0)
        {
            rows.Add(rowContent.ToArray());
        }

        return new InlineKeyboardMarkup(rows.ToArray());
    }

    public IReplyMarkup? BuildButtons<T>(T[][] buttons, Func<T, InlineKeyboardButton> mapperToButton)
    {
        var rows = new List<InlineKeyboardButton[]>();

        foreach (var row in buttons)
        {
            rows.Add(row.Select(b => mapperToButton(b)).ToArray());
        }

        return new InlineKeyboardMarkup(rows.ToArray());
    }

    public async Task LeaveCurrentChatAsync(ITelegramIncomingMessage update, string? message = null)
    {
        if (update is null) return;
        if (!update.IsGroup) throw new CannotLeavePrivateChannelException(update.ChatId!);
        await LeaveChatAsync(update.ChatId!, message);
    }

    public async Task KickAuthorFromChatAsync(ITelegramIncomingMessage update, string? message = null, DateTime? untilDate = null)
    {
        if (update is null) return;
        if (!update.IsGroup) throw new CannotKickFromPrivateChannelException(update.ChatId!);
        await KickUserFromChatAsync(update.AuthorUserId!, update.ChatId!, message, untilDate);
    }

    public async Task SendTextMessageToChatAsync(ITelegramIncomingMessage update, string message, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (update is not null && update.ChatId is not null)
            await SendTextMessageToChatAsync(update.ChatId, message, messageOptions);
    }

    public async Task SendTextMessageToAuthorAsync(ITelegramIncomingMessage update, string message, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (update is not null && update.AuthorUserId is not null)
            await SendTextMessageToUserAsync(update.AuthorUserId, message, messageOptions);
    }

    public async Task SendTextMessageToUserAsync(UserId userId, string message, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;
        try
        {
            await TelegramBotClient!.SendTextMessageAsync(userId.ToString(), message, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            await HandleSendingMessageExceptionAsync(new SendMessageToUserFailedException(ex.Message, userId, ex, messageOptions));
        }
    }

    public async Task SendTextMessageToChatAsync(ChatId chatId, string message, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;
        try
        {
            await TelegramBotClient!.SendTextMessageAsync(chatId.ToString(), message, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            await HandleSendingMessageExceptionAsync(new SendMessageToChatFailedException(ex.Message, chatId, ex, messageOptions));
        }
    }

    public async Task SendFileToUserAsync(UserId userId, Stream dataStream, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
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

            var file = new InputOnlineFile(dataStream, fileName);
            await TelegramBotClient!.SendDocumentAsync(userId.ToString(), file, message, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            await HandleSendingFileExceptionAsync(new SendFileToUserFailedException(ex.Message, userId, dataStream, fileName, ex, messageOptions));
        }
    }

    public async Task SendFileToChatAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
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

            var file = new InputOnlineFile(dataStream, fileName);
            await TelegramBotClient!.SendDocumentAsync(chatId.ToString(), file, message, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html);

        }
        catch (Exception ex)
        {
            await HandleSendingFileExceptionAsync(new SendFileToChatFailedException(ex.Message, chatId, dataStream, fileName, ex, messageOptions));
        }
    }

    public async Task SendFileToUserAsync(UserId userId, byte[] content, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToUserAsync(userId, dataStream, fileName, message, messageOptions);
    }

    public async Task SendFileToChatAsync(ChatId chatId, byte[] content, string fileName, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
    {
        using var dataStream = new MemoryStream(content);
        await SendFileToChatAsync(chatId, dataStream, fileName, message, messageOptions);
    }

    public async Task SendFileToUserAsync(UserId userId, string filePath, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
    {
        var content = System.IO.File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToUserAsync(userId, content, fileName, message, messageOptions);
    }

    public async Task SendFileToChatAsync(ChatId chatId, string filePath, string? message = null, ITelegramSendingMessageOptions? messageOptions = null)
    {
        var content = System.IO.File.ReadAllBytes(filePath);
        var fileName = new FileInfo(filePath).Name;
        await SendFileToChatAsync(chatId, content, fileName, message, messageOptions);
    }

    public async Task EditMessageAsync(ITelegramIncomingMessage update, string newMessageContent, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
            return;
        try
        {
            await TelegramBotClient!.EditMessageTextAsync(update.ChatId!.ToString(), update.MessageId!, newMessageContent, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, replyMarkup: null, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, update.MessageId!, update.ChatId!, ex, messageOptions));
        }
    }

    public async Task EditMessageAsync(ChatId chatId, MessageId messageId, string newMessageContent, ITelegramSendingMessageOptions? messageOptions = null)
    {
        if (string.IsNullOrWhiteSpace(newMessageContent))
            return;
        try
        {
            await TelegramBotClient!.EditMessageTextAsync(chatId.ToString(), messageId, newMessageContent, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, replyMarkup: null, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            await HandleEditMessageExceptionAsync(new EditMessageFailedException(ex.Message, messageId, chatId, ex, messageOptions));
        }
    }

    public async Task SimulateTypingAsync(ChatId chatId)
    {
        Logger.LogDebug("Start SimulateTypingAsync on {chatId}", chatId);
        await TelegramBotClient!.SendChatActionAsync(chatId.ToString(), ChatAction.Typing);
    }

    public async Task LeaveChatAsync(ChatId chatId, string? leavingMessage = null)
    {
        Logger.LogDebug("Leaving Chat : ChatId = {chatId}, Leaving Message = '{leavingMessage}'.", chatId, leavingMessage ?? "(none)");
        if (!string.IsNullOrEmpty(leavingMessage))
            await SendTextMessageToChatAsync(chatId, leavingMessage);
        await TelegramBotClient!.LeaveChatAsync(chatId.ToString());
    }

    public async Task KickUserFromChatAsync(UserId userId, ChatId chatId, string? kickingMessage = null, DateTime? untilDate = null)
    {
        Logger.LogDebug("Kicking User From Chat : UserId = {userId}, ChatId = {chatId}, Kicking Message = '{kickingMessage}', Until = {untilDate}", userId, chatId, kickingMessage ?? "(none)", untilDate?.ToLongTimeString() ?? "(none)");
        if (!string.IsNullOrEmpty(kickingMessage))
            await TelegramBotClient!.SendTextMessageAsync(chatId.ToString(), kickingMessage, parseMode: ParseMode.Html);
        await TelegramBotClient!.BanChatMemberAsync(chatId.ToString(), userId, untilDate.GetValueOrDefault());
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await StartAsync(cancellationToken).ConfigureAwait(false);
        await Task.Delay(-1, cancellationToken).ConfigureAwait(false);
        await StopAsync(CancellationToken.None);
    }

    protected async Task StopAsync(CancellationToken cancellationToken)
    {
        UnbindEvents();
        await TelegramBotClient!.CloseAsync(cancellationToken);
        Logger.LogInformation("Bot stopped.");
    }

    protected Task StartAsync(CancellationToken cancellationToken)
    {
        TelegramBotClient ??= new TelegramBotClient(Token);
        BindEvents();
        TelegramBotClient.StartReceiving(UpdateHandler, ErrorHandler, cancellationToken: cancellationToken);
        Logger.LogInformation("Bot started.");
        return Task.CompletedTask;
    }

    protected static ITelegramIncomingMessage GetAbstractIncomingMessage(Update update)
    {
        var genUpdate = new TelegramIncomingMessage(update);
        return genUpdate;
    }

    protected static Update? GetOriginObject(ITelegramIncomingMessage update)
    {
        if (update is null) return null;
        return update.OriginObject as Update;
    }

    protected async Task HandleUpdateAsync(ITelegramIncomingMessage update) => await HandleTelegramUpdateAsync((TelegramIncomingMessage)update);

    protected async Task HandleCommandUpdateAsync(ITelegramIncomingMessage update, string command, string[] commandArgs) => await HandleCommandUpdateAsync((TelegramIncomingMessage)update, command, commandArgs);

    protected async Task HandleMessageTextUpdateAsync(ITelegramIncomingMessage update) => await HandleMessageTextUpdateAsync((TelegramIncomingMessage)update);

    protected async Task HandleUpdateFailedAsync(ITelegramIncomingMessage update, Exception ex) => await HandleUpdateFailedAsync((TelegramIncomingMessage)update, ex);

    protected async Task HandleEventUpdateAsync(ITelegramIncomingMessage update)
    {
        if (update.ChatId is not null)
        {
            var origin = GetOriginObject(update);
            if (origin is null || origin.Message is null)
                return;
            if (origin.Message.ChannelChatCreated ?? false)
                await HandleChannelChatCreatedAsync((TelegramIncomingMessage)update);
            if (origin.Message.SupergroupChatCreated ?? false)
                await HandleSupergroupChatCreatedAsync((TelegramIncomingMessage)update);
            if (origin.Message.GroupChatCreated ?? false)
                await HandleGroupChatCreatedAsync((TelegramIncomingMessage)update);
            if (origin.Message.MigrateFromChatId > 0 && origin.Message.MigrateToChatId > 0)
                await HandleMigrationToSuperGroupAsync((TelegramIncomingMessage)update, origin.Message.MigrateFromChatId, origin.Message.MigrateToChatId);
            if (origin.Message.PinnedMessage is not null)
                await HandleMessagePinnedAsync((TelegramIncomingMessage)update, origin.Message.From!.Id, origin.Message.PinnedMessage.Text!);
            if (!string.IsNullOrEmpty(origin.Message.NewChatTitle))
                await HandleEventNewChatTitleAsync((TelegramIncomingMessage)update, origin.Message.NewChatTitle);
            if (origin.Message.NewChatMembers is not null)
            {
                foreach (var newChatMember in origin.Message.NewChatMembers)
                    if (newChatMember.Id == BotId)
                        await HandleEventBotJoinedAsync((TelegramIncomingMessage)update);
                    else
                        await HandleEventNewChatMemberAsync((TelegramIncomingMessage)update, newChatMember);
            }

            if (origin.Message.LeftChatMember is not null)
            {
                if (origin.Message.LeftChatMember.Id == BotId)
                    await HandleEventBotLeavedAsync((TelegramIncomingMessage)update);
                else
                    await HandleEventUserLeavedAsync((TelegramIncomingMessage)update, origin.Message.LeftChatMember.Id.ToString());
            }
        }
    }

    protected abstract Task HandleUpdateFailedAsync(TelegramIncomingMessage update, Exception ex);

    protected abstract Task HandleMessageTextUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleCommandUpdateAsync(TelegramIncomingMessage update, string command, string[] commandArgs);

    protected abstract Task HandleMessagePinnedAsync(TelegramIncomingMessage update, UserId pinnerUserId, string content);

    protected abstract Task HandleMigrationToSuperGroupAsync(TelegramIncomingMessage update, ChatId oldChatID, ChatId newChatID);

    protected abstract Task HandleChannelChatCreatedAsync(TelegramIncomingMessage update);

    protected abstract Task HandleSupergroupChatCreatedAsync(TelegramIncomingMessage update);

    protected abstract Task HandleGroupChatCreatedAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEventUserLeavedAsync(TelegramIncomingMessage update, UserId userID);

    protected abstract Task HandleEventBotLeavedAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEventBotJoinedAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEventNewChatMemberAsync(TelegramIncomingMessage update, User newChatMember);

    protected abstract Task HandleEventNewChatTitleAsync(TelegramIncomingMessage update, string newChatTitle);

    protected abstract Task HandleUnknownUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleInlineQueryUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleChosenInlineResultUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEditedMessageAsync(TelegramIncomingMessage update);

    protected abstract Task HandleCallbackQueryUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleChannelPostAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEditedChannelPostAsync(TelegramIncomingMessage update);

    protected abstract Task HandleShippingQueryUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandlePreCheckoutQueryUpdateAsync(TelegramIncomingMessage update);

    protected abstract Task HandleEditMessageExceptionAsync(EditMessageFailedException ex);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex);

    protected abstract Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex);

    protected abstract Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex);

    protected bool CheckIsBotTheSender(ITelegramIncomingMessage update)
    {
        if (update is null) return false;
        return BotId!.Equals(update.AuthorUserId);
    }

    protected async Task SimulateTypingAsync(ITelegramIncomingMessage update)
    {
        if (update is not null)
            await SimulateTypingAsync(update.ChatId!);
    }

    protected async Task HandleMessageUpdateAsync(ITelegramIncomingMessage update)
    {
        if (update.MessageText is null || string.IsNullOrEmpty(update.MessageText))
        {
            Logger.LogDebug("Received event {MessageId} from {AuthorUserId}.", update.MessageId, update.AuthorUserId);
            await HandleEventUpdateAsync(update);
            return;
        }

        if (update.AuthorUserId is null
            || update.MessageText is null
            || update.AuthorUserId.Equals(BotId))
        {
            return;
        }

        if (!update.MessageText.StartsWith(CommandStarterChar, StringComparison.InvariantCultureIgnoreCase))
        {
            Logger.LogDebug("Received message {MessageId} from {AuthorUserId}.", update.MessageId!, update.AuthorUserId!);
            await HandleMessageTextUpdateAsync(update);
            return;
        }

        var splitSpace = update.MessageText.Split(' ');
        if (splitSpace.Length >= 1 && (splitSpace.First().Contains("@" + BotName, StringComparison.InvariantCultureIgnoreCase) || !splitSpace[0].Contains('@')))
        {
            var command = splitSpace[0].Replace("@" + BotName, "")[CommandStarterChar.Length..];
            var commandArgs = splitSpace.Length > 1 ? splitSpace.Skip(1).ToArray() : Array.Empty<string>();
            if (commandArgs.Length > 0)
            {
                var args = string.Join(";", commandArgs);
                Logger.LogInformation("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with args '{Args}'.", update.MessageId, update.ChatId, update.AuthorUserId, command, args);
            }
            else
                Logger.LogInformation("Received message {MessageId} in {ChatId} from {AuthorUserId} with command '{Command}' with no args.", update.MessageId, update.ChatId, update.AuthorUserId, command);
            await HandleCommandUpdateAsync(update, command, commandArgs);
        }
        else
        {
            Logger.LogDebug("Received message {MessageId} from {AuthorUserId}.", update.MessageId, update.AuthorUserId);
            await HandleMessageTextUpdateAsync(update);
        }
    }

    private void BindEvents()
    {
        TelegramBotClient!.OnApiResponseReceived += OnApiResponseReceived;
        TelegramBotClient!.OnMakingApiRequest += OnMakingApiRequest;
    }

    private void UnbindEvents()
    {
        TelegramBotClient!.OnApiResponseReceived -= OnApiResponseReceived;
        TelegramBotClient!.OnMakingApiRequest -= OnMakingApiRequest;
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Logger.LogError(exception, "Error while receiving message.");
        return Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
     => await HandleUpdateAsync(new TelegramIncomingMessage(update));

    private async ValueTask OnMakingApiRequest(ITelegramBotClient botClient, global::Telegram.Bot.Args.ApiRequestEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        if (eventArgs.HttpRequestMessage?.Content is null)
        {
            return;
        }

        var content = await eventArgs.HttpRequestMessage!.Content!.ReadAsStringAsync(cancellationToken) ?? "Empty body";
        Logger.LogDebug("{MethodName} : REQUEST with {Body}", eventArgs.Request.MethodName, content);
    }

    private async ValueTask OnApiResponseReceived(ITelegramBotClient botClient, global::Telegram.Bot.Args.ApiResponseEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        if (eventArgs.ResponseMessage?.Content is null)
        {
            return;
        }

        var content = await eventArgs.ResponseMessage!.Content!.ReadAsStringAsync(cancellationToken) ?? "Empty body";
        Logger.LogDebug("{MethodName} : RESPONSE {HttpStatus} with {Body}", eventArgs.ApiRequestEventArgs.Request.MethodName, eventArgs.ResponseMessage.StatusCode, content);
    }

    private async Task HandleTelegramUpdateAsync(TelegramIncomingMessage telegramIncomingMessage)
    {
        Logger.LogDebug("Handling Telegram Update {MessageId} of type {MessageType}...", telegramIncomingMessage.MessageId, telegramIncomingMessage.MessageType);
        switch (telegramIncomingMessage.MessageType)
        {
            case UpdateType.Message:
                try
                {
                    await HandleMessageUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.Unknown:
                try
                {
                    await HandleUnknownUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.InlineQuery:
                try
                {
                    await HandleInlineQueryUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.ChosenInlineResult:
                try
                {
                    await HandleChosenInlineResultUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.EditedMessage:
                try
                {
                    await HandleEditedMessageAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.CallbackQuery:
                try
                {
                    await HandleCallbackQueryUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.ChannelPost:
                try
                {
                    await HandleChannelPostAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.EditedChannelPost:
                try
                {
                    await HandleEditedChannelPostAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.ShippingQuery:
                try
                {
                    await HandleShippingQueryUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            case UpdateType.PreCheckoutQuery:
                try
                {
                    await HandlePreCheckoutQueryUpdateAsync(telegramIncomingMessage);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex);
                }
                break;
            default:
                break;
        }
    }
}
