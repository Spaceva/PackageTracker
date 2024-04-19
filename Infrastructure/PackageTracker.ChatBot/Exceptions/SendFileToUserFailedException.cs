namespace PackageTracker.ChatBot;

public class SendFileToUserFailedException(string message, UserId userId, Stream dataStream, string streamedDataFileName, Exception innerException, ISendingMessageOptions? messageOptions = null) : Exception(message, innerException)
{
    public UserId UserId => userId;
    public long DataStreamLength => dataStream.Length;
    public string StreamedDataFileName => streamedDataFileName;
    public ISendingMessageOptions? SendingMessageOptions => messageOptions;
}
