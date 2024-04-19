using Discord;
using Discord.WebSocket;

namespace PackageTracker.ChatBot.Discord;

public class DiscordIncomingMessage : IncomingMessage
{
    public DiscordIncomingMessage(SocketMessage message)
    {
        IsGroup = message.Channel is IGuildChannel;
        MessageText = message.Content;
        MessageId = message.Id;
        AuthorUserId = message.Author.Id;
        ChatId = message.Channel.Id;
        ChatTitle = message.Channel.Name;
        OriginObject = message;
        AuthorUserName = message.Author.Username;
        var embed = message.Embeds.SingleOrDefault();
        var attachment = message.Attachments.SingleOrDefault();
        if (embed is not null)
        {
            Attachment = new DiscordAttachment(embed);
        }
        else if (attachment is not null)
        {
            Attachment = new DiscordAttachment(attachment);
        }
    }
}
