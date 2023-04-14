using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ChatId = PackageTracker.Telegram.SDK.Convertibles.ChatId;
using MessageId = PackageTracker.Telegram.SDK.Convertibles.MessageId;

namespace PackageTracker.Telegram.SDK.Base;

internal class TelegramIncomingMessage : ITelegramIncomingMessage
{
    public bool IsGroup { get; protected set; }
    public MessageId? MessageId { get; protected set; }
    public ChatId? ChatId { get; protected set; }
    public string? ChatTitle { get; protected set; }
    public UserId? AuthorUserId { get; protected set; }
    public string? AuthorUserName { get; protected set; }
    public string? MessageText { get; protected set; }
    public object? OriginObject { get; protected set; }
    public bool IsUserAdminOfChat { get; protected set; }
    public IAttachment? Attachment { get; protected set; }

    public bool IsSuperGroup { get; protected set; }
    public bool IsForwarded { get; protected set; }
    public UserId? ForwardedFromUserId { get; protected set; }
    public string? CallBackData { get; protected set; }
    public UserId? CallBackOriginalMessageUserId { get; protected set; }
    public string? CallBackOriginalMessageUserName { get; protected set; }
    public string? TextBeforeEdit { get; protected set; }

    public InlineQuery? InlineQuery { get; protected set; }

    public string? ChosenInlineResultId { get; protected set; }
    public User? ChosenInlineResultUserFrom { get; protected set; }
    public Location? ChosenInlineResultLocation { get; protected set; }
    public MessageId? ChosenInlineResultInlineMessageId { get; protected set; }
    public string? ChosenInlineResultQuery { get; protected set; }
    public ChosenInlineResult? ChosenInlineResult { get; protected set; }
    public UpdateType MessageType { get; protected set; }

    public TelegramIncomingMessage(Message message)
    {
        HydrateFromMessage(message);
        OriginObject = message;
        MessageType = UpdateType.Message;
    }

    public TelegramIncomingMessage(Update tgUpdate)
    {
        HydrateFromCallbackQuery(tgUpdate.CallbackQuery);
        HydrateFromMessage(tgUpdate.Message);
        HydrateFromEditedMessage(tgUpdate.EditedMessage);
        HydrateFromChosenInlineResult(tgUpdate.ChosenInlineResult);
        InlineQuery = tgUpdate.InlineQuery;
        OriginObject = tgUpdate;
        MessageType = tgUpdate.Type;
    }

    public TelegramIncomingMessage(Update tgUpdate, User overrideAuthor) : this(tgUpdate)
    {
        if (tgUpdate?.Message is null)
        {
            return;
        }

        AuthorUserName = overrideAuthor.Username;
        AuthorUserId = overrideAuthor.Id;
    }

    public TelegramIncomingMessage(TelegramIncomingMessage copiedMessage)
    {
        IsGroup = copiedMessage.IsGroup;
        MessageId = copiedMessage.MessageId;
        ChatId = copiedMessage.ChatId;
        ChatTitle = copiedMessage.ChatTitle;
        AuthorUserId = copiedMessage.AuthorUserId;
        AuthorUserName = copiedMessage.AuthorUserName;
        MessageText = copiedMessage.MessageText;
        OriginObject = copiedMessage.OriginObject;
        IsUserAdminOfChat = copiedMessage.IsUserAdminOfChat;
        Attachment = copiedMessage.Attachment;
        IsSuperGroup = copiedMessage.IsSuperGroup;
        IsForwarded = copiedMessage.IsForwarded;
        ForwardedFromUserId = copiedMessage.ForwardedFromUserId;
        CallBackData = copiedMessage.CallBackData;
        CallBackOriginalMessageUserId = copiedMessage.CallBackOriginalMessageUserId;
        CallBackOriginalMessageUserName = copiedMessage.CallBackOriginalMessageUserName;
    }

    public TelegramIncomingMessage(InlineQuery inlineQuery)
    {
        OriginObject = inlineQuery;
        InlineQuery = inlineQuery;
        MessageType = UpdateType.InlineQuery;
    }

    public TelegramIncomingMessage(ChosenInlineResult chosenInlineResult)
    {
        ChosenInlineResult = chosenInlineResult;
        OriginObject = chosenInlineResult;
        MessageType = UpdateType.ChosenInlineResult;
    }

    private void HydrateFromMessage(Message? message)
    {
        if (message is null)
        {
            return;
        }

        MessageId = message.MessageId;
        ChatId = message.Chat.Id;
        MessageText = message.Text;
        IsGroup = message.Chat.Type == ChatType.Group || message.Chat.Type == ChatType.Supergroup;
        IsSuperGroup = message.Chat.Type == ChatType.Supergroup;
        ChatTitle = message.Chat.Title;

        if (message.From is not null)
        {
            AuthorUserId = message.From.Id;
            AuthorUserName = message.From.Username;
        }

        if (message.ForwardFrom is not null)
        {
            IsForwarded = true;
            ForwardedFromUserId = message.ForwardFrom.Id;
        }
    }

    private void HydrateFromEditedMessage(Message? message)
    {
        if (message is not null)
        {
            TextBeforeEdit = message.Text;
        }
    }

    private void HydrateFromCallbackQuery(CallbackQuery? callbackQuery)
    {
        if (callbackQuery is null)
        {
            return;
        }

        CallBackData = callbackQuery.Data;
        AuthorUserId = callbackQuery.From.Id;
        AuthorUserName = callbackQuery.From.Username;

        if (callbackQuery.Message is null)
        {
            return;
        }

        MessageId = callbackQuery.Message.MessageId;
        MessageText = callbackQuery.Message.Text;
        ChatId = callbackQuery.Message.Chat.Id;
        IsGroup = callbackQuery.Message.Chat.Type == ChatType.Group || callbackQuery.Message.Chat.Type == ChatType.Supergroup;
        IsSuperGroup = callbackQuery.Message.Chat.Type == ChatType.Supergroup;
        ChatTitle = callbackQuery.Message.Chat.Title;

        if (callbackQuery.Message.From is not null)
        {
            CallBackOriginalMessageUserId = callbackQuery.Message.From.Id;
            CallBackOriginalMessageUserName = callbackQuery.Message.From.Username;
        }

        if (callbackQuery.Message.ForwardFrom is not null)
        {
            IsForwarded = true;
            ForwardedFromUserId = callbackQuery.Message.ForwardFrom.Id.ToString();
        }
    }

    private void HydrateFromChosenInlineResult(ChosenInlineResult? chosenInlineResult)
    {
        if (chosenInlineResult is null)
        {
            return;
        }

        ChosenInlineResultId = chosenInlineResult.ResultId;
        ChosenInlineResultUserFrom = chosenInlineResult.From;
        ChosenInlineResultLocation = chosenInlineResult.Location;
        ChosenInlineResultQuery = chosenInlineResult.Query;

        if (chosenInlineResult.InlineMessageId is not null)
        {
            ChosenInlineResultInlineMessageId = chosenInlineResult.InlineMessageId;
        }
    }
}
