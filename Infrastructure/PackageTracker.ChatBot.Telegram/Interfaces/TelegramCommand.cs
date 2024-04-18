namespace PackageTracker.ChatBot.Telegram;
public abstract class TelegramCommand
{
    public TelegramIncomingMessage MessageProperties { get; init; } = default!;
}
