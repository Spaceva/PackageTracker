using Microsoft.Extensions.Logging;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PackageTracker.ChatBot.Telegram;

public abstract class TelegramChatBot(IServiceProvider serviceProvider) : ChatBot<Update, TelegramIncomingMessage, TelegramSendingMessageOptions, TelegramCommand>(serviceProvider), ITelegramBot
{
    protected int offset = 0;

    private string? botName;

    private UserId? botID;

    private bool isReady = false;

    public override sealed string BeginBoldTag => "<b>";
    public override sealed string EndBoldTag => "</b>";
    public override sealed string BeginItalicTag => "<i>";
    public override sealed string EndItalicTag => "</i>";
    public override sealed string CommandStarterChar => "/";
    public override sealed string BotName
    {
        get
        {
            if (TelegramBotClient is null)
            {
                return base.BotName;
            }

            if (string.IsNullOrEmpty(botName))
            {
                botName = TelegramBotClient.GetMeAsync().GetAwaiter().GetResult().Username!;
            }
            return botName;
        }
    }
    public override sealed UserId BotId
    {
        get
        {
            if (TelegramBotClient is null)
            {
                return default!;
            }

            if (botID is null || string.IsNullOrEmpty(botID))
            {
                botID = TelegramBotClient!.GetMeAsync().GetAwaiter().GetResult().Id!;
            }

            return botID;
        }
    }

    public sealed override bool IsReady => isReady;

    protected ITelegramBotClient? TelegramBotClient { get; private set; }

    public async Task<UserId> GetChatCreatorAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        var admins = await GetChatAdminsAsync(chatId, cancellationToken);
        return admins.Single(u => u.Value == (int)ChatMemberStatus.Creator).Key;
    }

    public async Task<Dictionary<UserId, ChatPermission>> GetChatAdminsAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        var admins = await TelegramBotClient!.GetChatAdministratorsAsync(chatId.ToString(), cancellationToken: cancellationToken);
        return admins.ToDictionary(kvp => (UserId)kvp.User.Id.ToString(), kvp => (ChatPermission)(int)kvp.Status);
    }

    public async Task<ChatPermission?> GetChatPermissionAsync(ChatId chatId, UserId userId, CancellationToken cancellationToken = default)
    {
        var rights = await TelegramBotClient!.GetChatMemberAsync(chatId.ToString(), userId, cancellationToken: cancellationToken);
        return (int?)rights?.Status;
    }

    public async Task SendLocationAsync(ChatId chatId, float lat, float lng, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Sending Location (lat:{lat},lng:{lng}) to Chat {chatId}.", lat, lng, chatId);
        await TelegramBotClient!.SendLocationAsync(chatId.ToString(), lat, lng, cancellationToken: cancellationToken);
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
        int divisor = (buttons.Length % 3 == 0 || buttons.Length % 3 == 2) ? 3 : 2;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > 0 && i % divisor == 0)
            {
                rows.Add([.. rowContent]);
                rowContent = [];
            }
            rowContent.Add(mapperToButton(buttons[i]));
        }

        if (rowContent.Count > 0)
        {
            rows.Add([.. rowContent]);
        }

        return new InlineKeyboardMarkup(rows.ToArray());
    }

    public IReplyMarkup? BuildButtons<T>(IEnumerable<IEnumerable<T>> buttons, Func<T, InlineKeyboardButton> mapperToButton)
    {
        var rows = new List<InlineKeyboardButton[]>();

        foreach (var row in buttons)
        {
            rows.Add(row.Select(b => mapperToButton(b)).ToArray());
        }

        return new InlineKeyboardMarkup(rows.ToArray());
    }

    protected override sealed async Task StopAsync(CancellationToken cancellationToken)
    {
        isReady = false;
        UnbindEvents();
        try
        {
            await TelegramBotClient!.CloseAsync(cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            if (exception.ErrorCode != (int)HttpStatusCode.TooManyRequests)
            {
                throw;
            }
        }

        Logger.LogInformation("{BotName} stopped.", BotName);
    }

    protected override sealed Task StartAsync(CancellationToken cancellationToken)
    {
        TelegramBotClient ??= new TelegramBotClient(Token);
        BindEvents();
        TelegramBotClient.StartReceiving(UpdateHandler, ErrorHandler, cancellationToken: cancellationToken);
        Logger.LogInformation("{BotName} started.", BotName);
        isReady = true;
        return Task.CompletedTask;
    }

    protected override sealed TelegramIncomingMessage ParseIncomingMessage(Update incomingMessage)
    {
        var genUpdate = new TelegramIncomingMessage(incomingMessage);
        return genUpdate;
    }

    protected override sealed async Task SimulateTypingInternalAsync(ChatId chatId, CancellationToken cancellationToken = default) 
        => await TelegramBotClient!.SendChatActionAsync(chatId.ToString(), ChatAction.Typing, cancellationToken: cancellationToken);

    protected override sealed async Task HandleUpdateAsync(IIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
    {
        TelegramIncomingMessage telegramIncomingMessage = (TelegramIncomingMessage)incomingMessage;
        Logger.LogDebug("Handling Telegram Update {MessageId} of type {MessageType}...", telegramIncomingMessage.MessageId, telegramIncomingMessage.MessageType);
        switch (telegramIncomingMessage.MessageType)
        {
            case UpdateType.Message:
                try
                {
                    await HandleMessageUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.Unknown:
                try
                {
                    await HandleUnknownUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.InlineQuery:
                try
                {
                    await HandleInlineQueryUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.ChosenInlineResult:
                try
                {
                    await HandleChosenInlineResultUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.EditedMessage:
                try
                {
                    await HandleEditedMessageAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.CallbackQuery:
                try
                {
                    await HandleCallbackQueryUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.ChannelPost:
                try
                {
                    await HandleChannelPostUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.EditedChannelPost:
                try
                {
                    await HandleEditedChannelPostAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.ShippingQuery:
                try
                {
                    await HandleShippingQueryUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.PreCheckoutQuery:
                try
                {
                    await HandlePreCheckoutQueryUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.Poll:
                try
                {
                    await HandlePollUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.PollAnswer:
                try
                {
                    await HandlePollAnswerUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.MyChatMember:
                try
                {
                    await HandleMyChatMemberUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.ChatMember:
                try
                {
                    await HandleChatMemberUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            case UpdateType.ChatJoinRequest:
                try
                {
                    await HandleChatJoinRequestUpdateAsync(telegramIncomingMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleUpdateFailedAsync(telegramIncomingMessage, ex, cancellationToken);
                }
                break;
            default:
                break;
        }
    }

    protected override sealed async Task HandleEventUpdateInternalAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
    {
        if (incomingMessage.ChatId is not null)
        {
            var origin = GetOriginObject(incomingMessage);
            if (origin is null || origin.Message is null)
            {
                return;
            }

            if (origin.Message.ChannelChatCreated ?? false)
            {
                await HandleChannelChatCreatedAsync(incomingMessage);
            }

            if (origin.Message.SupergroupChatCreated ?? false)
            {
                await HandleSupergroupChatCreatedAsync(incomingMessage);
            }

            if (origin.Message.GroupChatCreated ?? false)
            {
                await HandleGroupChatCreatedAsync(incomingMessage);
            }

            if (origin.Message.MigrateFromChatId > 0 && origin.Message.MigrateToChatId > 0)
            {
                await HandleMigrationToSuperGroupAsync(incomingMessage, origin.Message.MigrateFromChatId, origin.Message.MigrateToChatId);
            }

            if (origin.Message.PinnedMessage is not null)
            {
                await HandleMessagePinnedAsync(incomingMessage, origin.Message.From!.Id, origin.Message.PinnedMessage.Text!);
            }

            if (!string.IsNullOrEmpty(origin.Message.NewChatTitle))
            {
                await HandleEventNewChatTitleAsync(incomingMessage, origin.Message.NewChatTitle);
            }

            if (origin.Message.NewChatMembers is not null)
            {
                foreach (var newChatMember in origin.Message.NewChatMembers)
                {
                    if (newChatMember.Id == BotId)
                    {
                        await HandleEventBotJoinedAsync(incomingMessage);
                    }
                    else
                    {
                        await HandleEventNewChatMemberAsync(incomingMessage, newChatMember);
                    }
                }
            }

            if (origin.Message.LeftChatMember is not null)
            {
                if (origin.Message.LeftChatMember.Id == BotId)
                {
                    await HandleEventBotLeavedAsync(incomingMessage);
                }
                else
                {
                    await HandleEventUserLeavedAsync(incomingMessage, origin.Message.LeftChatMember.Id.ToString());
                }
            }
        }
    }

    protected override sealed async Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, TelegramSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) => await TelegramBotClient!.SendTextMessageAsync(chatId.ToString(), message, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html, cancellationToken: cancellationToken);

    protected override sealed async Task SendTextMessageToUserInternalAsync(UserId userId, string message, TelegramSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default) => await TelegramBotClient!.SendTextMessageAsync(userId.ToString(), message, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html, cancellationToken: cancellationToken);

    protected override sealed async Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, TelegramSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        var file = new InputFileStream(dataStream, fileName);
        await TelegramBotClient!.SendDocumentAsync(chatId.ToString(), file, caption: message, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
    }

    protected override sealed async Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, TelegramSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        var file = new InputFileStream(dataStream, fileName);
        await TelegramBotClient!.SendDocumentAsync(userId.ToString(), file, caption: message, disableNotification: messageOptions is not null && messageOptions.DisableNotification, replyToMessageId: messageOptions is not null ? messageOptions.ReplyToMessageId : 0, replyMarkup: messageOptions?.ReplyMarkup, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
    }

    protected override sealed async Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, TelegramSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
        => await TelegramBotClient!.EditMessageTextAsync(chatId.ToString(), messageId, newMessageContent, disableWebPagePreview: messageOptions is not null && messageOptions.DisableWebPagePreview, replyMarkup: null, parseMode: ParseMode.Html, cancellationToken: cancellationToken);

    protected abstract Task HandleMessagePinnedAsync(TelegramIncomingMessage incomingMessage, UserId pinnerUserId, string content);

    protected abstract Task HandleMigrationToSuperGroupAsync(TelegramIncomingMessage incomingMessage, ChatId oldChatID, ChatId newChatID);

    protected abstract Task HandleChannelChatCreatedAsync(TelegramIncomingMessage incomingMessage);

    protected abstract Task HandleSupergroupChatCreatedAsync(TelegramIncomingMessage incomingMessage);

    protected abstract Task HandleGroupChatCreatedAsync(TelegramIncomingMessage incomingMessage);

    protected abstract Task HandleEventUserLeavedAsync(TelegramIncomingMessage incomingMessage, UserId userID);

    protected abstract Task HandleEventBotLeavedAsync(TelegramIncomingMessage incomingMessage);

    protected abstract Task HandleEventBotJoinedAsync(TelegramIncomingMessage incomingMessage);

    protected abstract Task HandleEventNewChatMemberAsync(TelegramIncomingMessage incomingMessage, User newChatMember);

    protected abstract Task HandleEventNewChatTitleAsync(TelegramIncomingMessage incomingMessage, string newChatTitle);

    protected abstract Task HandleUnknownUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleInlineQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleChosenInlineResultUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleEditedMessageAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleCallbackQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleChannelPostUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleEditedChannelPostAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleShippingQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandlePreCheckoutQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandlePollUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleChatJoinRequestUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleChatMemberUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandleMyChatMemberUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default);

    protected abstract Task HandlePollAnswerUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default);

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

    private async Task UpdateHandler(ITelegramBotClient botClient, Update incomingMessage, CancellationToken cancellationToken)
     => await HandleUpdateAsync(new TelegramIncomingMessage(incomingMessage), cancellationToken);

    private async ValueTask OnMakingApiRequest(ITelegramBotClient botClient, global::Telegram.Bot.Args.ApiRequestEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        var content = eventArgs.HttpRequestMessage?.Content is not null ? await eventArgs.HttpRequestMessage.Content.ReadAsStringAsync(cancellationToken) : "Empty body";
        Logger.LogDebug("{MethodName} : REQUEST with {Body}", eventArgs.Request.MethodName, content);
    }

    private async ValueTask OnApiResponseReceived(ITelegramBotClient botClient, global::Telegram.Bot.Args.ApiResponseEventArgs eventArgs, CancellationToken cancellationToken = default)
    {
        var content = eventArgs.ResponseMessage.Content is not null ? await eventArgs.ResponseMessage.Content.ReadAsStringAsync(cancellationToken) : "Empty body";
        Logger.LogDebug("{MethodName} : RESPONSE {HttpStatus} with {Body}", eventArgs.ApiRequestEventArgs.Request.MethodName, eventArgs.ResponseMessage.StatusCode, content);
    }
}
