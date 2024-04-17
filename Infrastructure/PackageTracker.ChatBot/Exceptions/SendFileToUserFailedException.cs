namespace PackageTracker.ChatBot;

[Serializable]
public class SendFileToUserFailedException : Exception
{
    public UserId UserId { get; }
    public long DataStreamLength { get; }
    public string StreamedDataFileName { get; }
    public ISendingMessageOptions? SendingMessageOptions { get; }

    public SendFileToUserFailedException(UserId userId, Stream dataStream, string streamedDataFileName, ISendingMessageOptions? messageOptions = null)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToUserFailedException(string message, UserId userId, Stream dataStream, string streamedDataFileName, ISendingMessageOptions? messageOptions = null) : base(message)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToUserFailedException(string message, UserId userId, Stream dataStream, string streamedDataFileName, Exception innerException, ISendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }
}
