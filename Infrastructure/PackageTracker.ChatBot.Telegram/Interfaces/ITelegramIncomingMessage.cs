using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PackageTracker.ChatBot.Telegram;

public interface ITelegramIncomingMessage
{
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
