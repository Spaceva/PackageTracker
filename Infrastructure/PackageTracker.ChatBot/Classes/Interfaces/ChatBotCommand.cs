namespace PackageTracker.ChatBot;

public abstract class ChatBotCommand<TIncomingMessage> : IChatBotCommand where TIncomingMessage : IIncomingMessage
{
    public TIncomingMessage MessageProperties { get; init; } = default!;

    IIncomingMessage IChatBotCommand.MessageProperties => MessageProperties;
}
