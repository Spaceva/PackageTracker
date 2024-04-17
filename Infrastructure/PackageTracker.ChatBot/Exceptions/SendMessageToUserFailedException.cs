namespace PackageTracker.ChatBot;

[Serializable]
public class SendMessageToUserFailedException : Exception
{
    public UserId UserId { get; }
    public ISendingMessageOptions? SendingMessageOptions { get; }

    public SendMessageToUserFailedException(UserId userId, ISendingMessageOptions? messageOptions = null)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToUserFailedException(string message, UserId userId, ISendingMessageOptions? messageOptions = null) : base(message)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }

    public SendMessageToUserFailedException(string message, UserId userId, Exception innerException, ISendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
    }
}
