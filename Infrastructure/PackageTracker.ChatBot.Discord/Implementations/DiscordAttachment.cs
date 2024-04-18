using Discord;

namespace PackageTracker.ChatBot.Discord;

public class DiscordAttachment : IAttachment
{
    public string? Filename { get; set; }

    public string? Url { get; set; }

    public string? ProxyUrl { get; set; }

    public int Size { get; set; }

    public DiscordAttachment() { }

    public DiscordAttachment(IEmbed embed)
    {
        Url = embed.Url;
        Filename = embed.Title;
        if (embed.Image.HasValue)
        {
            ProxyUrl = embed.Image.Value.ProxyUrl;
        }
    }

    public DiscordAttachment(Attachment attachment)
    {
        Url = attachment.Url;
        Filename = attachment.Filename;
        ProxyUrl = attachment.ProxyUrl;
        Size = attachment.Size;
    }
}
