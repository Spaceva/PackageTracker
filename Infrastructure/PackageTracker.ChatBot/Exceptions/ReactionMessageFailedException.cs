namespace PackageTracker.ChatBot;

[Serializable]
public class ReactionMessageFailedException : Exception
{
    public MessageId MessageId { get; private set; }
    public ChatId ChatId { get; private set; }
    public IEmoji Emoji { get; private set; }

    public ReactionMessageFailedException(MessageId messageId, ChatId chatId, IEmoji emoji) { MessageId = messageId; ChatId = chatId; Emoji = emoji; }
    public ReactionMessageFailedException(string message, MessageId messageId, ChatId chatId, IEmoji emoji) : base(message) { MessageId = messageId; ChatId = chatId; Emoji = emoji; }
    public ReactionMessageFailedException(string message, MessageId messageId, ChatId chatId, Exception innerException, IEmoji emoji) : base(message, innerException) { MessageId = messageId; ChatId = chatId; Emoji = emoji; }
}
