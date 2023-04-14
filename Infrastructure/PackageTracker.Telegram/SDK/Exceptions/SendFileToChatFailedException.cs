using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class SendFileToChatFailedException : Exception
{
    public ChatId ChatId { get; }
    public long DataStreamLength { get; }
    public string StreamedDataFileName { get; }
    public ITelegramSendingMessageOptions? SendingMessageOptions { get; }

    public SendFileToChatFailedException(ChatId chatId, Stream dataStream, string streamedDataFileName, ITelegramSendingMessageOptions? messageOptions = null)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToChatFailedException(string message, ChatId chatId, Stream dataStream, string streamedDataFileName, ITelegramSendingMessageOptions? messageOptions = null) : base(message)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToChatFailedException(string message, ChatId chatId, Stream dataStream, string streamedDataFileName, Exception innerException, ITelegramSendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        ChatId = chatId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }
}
