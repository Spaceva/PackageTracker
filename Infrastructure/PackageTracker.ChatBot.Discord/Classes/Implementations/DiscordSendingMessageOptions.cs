using Discord;

namespace PackageTracker.ChatBot.Discord;

public class DiscordSendingMessageOptions : ISendingMessageOptions
{
    public bool IsTTS { get; set; } = false;
    public Embed? Embed { get; set; } = null;
    public RequestOptions? RequestOptions { get; set; } = null;
}
