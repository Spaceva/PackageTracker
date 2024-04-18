using Discord;

namespace PackageTracker.ChatBot.Discord;

public sealed class DiscordEmoji(string emoji) : IEmoji, IEmote
{
    public string Name => emoji;

    public string Emoji => Name;
}
