namespace PackageTracker.ChatBot.Discord;

public abstract class DiscordCommand : ChatBotCommand<DiscordIncomingMessage>
{
    public ulong? GuildId { get; init; }
}
