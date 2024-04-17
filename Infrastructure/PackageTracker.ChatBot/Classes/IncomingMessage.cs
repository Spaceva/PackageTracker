namespace PackageTracker.ChatBot;

public class IncomingMessage : IIncomingMessage
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

    protected IncomingMessage() { }

    public IncomingMessage(IncomingMessage messageToCopy)
    {
        IsGroup = messageToCopy.IsGroup;
        MessageId = messageToCopy.MessageId;
        ChatId = messageToCopy.ChatId;
        ChatTitle = messageToCopy.ChatTitle;
        AuthorUserId = messageToCopy.AuthorUserId;
        AuthorUserName = messageToCopy.AuthorUserName;
        MessageText = messageToCopy.MessageText;
        OriginObject = messageToCopy.OriginObject;
        IsUserAdminOfChat = messageToCopy.IsUserAdminOfChat;
        Attachment = messageToCopy.Attachment;
    }
}
