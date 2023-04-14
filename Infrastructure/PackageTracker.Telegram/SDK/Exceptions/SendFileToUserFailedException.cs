using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Exceptions;

[Serializable]
internal class SendFileToUserFailedException : Exception
{
    public UserId UserId { get; }
    public long DataStreamLength { get; }
    public string StreamedDataFileName { get; }
    public ITelegramSendingMessageOptions? SendingMessageOptions { get; }

    public SendFileToUserFailedException(UserId userId, Stream dataStream, string streamedDataFileName, ITelegramSendingMessageOptions? messageOptions = null)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToUserFailedException(string message, UserId userId, Stream dataStream, string streamedDataFileName, ITelegramSendingMessageOptions? messageOptions = null) : base(message)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }

    public SendFileToUserFailedException(string message, UserId userId, Stream dataStream, string streamedDataFileName, Exception innerException, ITelegramSendingMessageOptions? messageOptions = null) : base(message, innerException)
    {
        UserId = userId;
        SendingMessageOptions = messageOptions;
        DataStreamLength = dataStream.Length;
        StreamedDataFileName = streamedDataFileName;
    }
}
