namespace PackageTracker.ChatBot;

public class SendMessageToUserFailedException(string message, UserId userId, Exception innerException, ISendingMessageOptions? messageOptions = null) : Exception(message, innerException)
{
    public UserId UserId => userId;
    public ISendingMessageOptions? SendingMessageOptions => messageOptions;
}
