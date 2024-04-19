namespace PackageTracker.ChatBot;

public interface IChatBotCommand
{
    public IIncomingMessage MessageProperties { get; }
}
