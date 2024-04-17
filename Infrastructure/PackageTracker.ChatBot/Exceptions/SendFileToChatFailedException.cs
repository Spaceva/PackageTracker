namespace PackageTracker.ChatBot;

[Serializable]
public class SendFileToChatFailedException : Exception
{
    public ChatId ChatId { get; }
    public long DataStreamLength { get; }
    public string StreamedDataFileName { get; }
    public ISendingMessageOptions? SendingMessageOptions { get; }

    public SendFileToChatFailedException(ChatId chatId, Stream dataStream, string streamedDataFileName, ISendingMessageOptions? messageOptions = null)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToChatFailedException(string message, ChatId chatId, Stream dataStream, string streamedDataFileName, ISendingMessageOptions? messageOptions = null) : base(message)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToChatFailedException(string message, ChatId chatId, Stream dataStream, string streamedDataFileName, Exception innerException, ISendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }
}
