namespace PackageTracker.ChatBot;

public class SendFileToChatFailedException(string message, ChatId chatId, Stream dataStream, string streamedDataFileName, Exception innerException, ISendingMessageOptions? messageOptions = null) : Exception(message, innerException)
{
    public ChatId ChatId => chatId;
    public long DataStreamLength => dataStream.Length;
    public string StreamedDataFileName => streamedDataFileName;
    public ISendingMessageOptions? SendingMessageOptions => messageOptions;
}
