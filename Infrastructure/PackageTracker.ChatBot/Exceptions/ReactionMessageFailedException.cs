namespace PackageTracker.ChatBot;

public class ReactionMessageFailedException(string message, MessageId messageId, ChatId chatId, Exception innerException, IEmoji emoji) : Exception(message, innerException)
{
    public MessageId MessageId => messageId; 
    public ChatId ChatId => chatId; 
    public IEmoji Emoji => emoji;
}
