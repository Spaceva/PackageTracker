using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using PackageTracker.Telegram.SDK.Convertibles;
using MessageId = PackageTracker.Telegram.SDK.Convertibles.MessageId;
using ChatId = PackageTracker.Telegram.SDK.Convertibles.ChatId;

namespace PackageTracker.Telegram.SDK.Interfaces;

internal interface ITelegramIncomingMessage
{
    bool IsGroup { get; }
    MessageId? MessageId { get; }
    ChatId? ChatId { get; }
    string? ChatTitle { get; }
    UserId? AuthorUserId { get; }
    string? AuthorUserName { get; }
    string? MessageText { get; }
    object? OriginObject { get; }
    bool IsUserAdminOfChat { get; }
    IAttachment? Attachment { get; }
    bool IsSuperGroup { get; }
    bool IsForwarded { get; }
    UserId? ForwardedFromUserId { get; }
    string? CallBackData { get; }
    UserId? CallBackOriginalMessageUserId { get; }
    string? CallBackOriginalMessageUserName { get; }
    string? ChosenInlineResultId { get; }
    User? ChosenInlineResultUserFrom { get; }
    Location? ChosenInlineResultLocation { get; }
    MessageId? ChosenInlineResultInlineMessageId { get; }
    string? ChosenInlineResultQuery { get; }
    UpdateType MessageType { get; }
    InlineQuery? InlineQuery { get; }
    ChosenInlineResult? ChosenInlineResult { get; }
}
