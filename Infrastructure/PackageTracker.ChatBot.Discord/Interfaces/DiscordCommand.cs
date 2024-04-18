namespace PackageTracker.ChatBot.Discord;

public abstract class DiscordCommand
{
    public DiscordIncomingMessage MessageProperties { get; init; } = default!;

    public ulong? GuildId { get; init; }
}
