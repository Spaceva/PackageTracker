namespace PackageTracker.ChatBot;

public interface IIncomingMessage
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
}
