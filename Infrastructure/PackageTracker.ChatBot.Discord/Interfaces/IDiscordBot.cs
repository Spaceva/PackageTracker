using Discord.WebSocket;

namespace PackageTracker.ChatBot.Discord;
public interface IDiscordBot : IChatBot
{
    SocketGuildChannel? GetChannel(string guildName, string channelName);
    SocketGuildChannel? GetChannel(string guildName, ulong channelID);
    SocketGuildChannel? GetChannel(ulong guildId, string channelName);
    SocketGuildChannel? GetChannel(ulong guildId, ulong channelID);
    SocketGuild? GetGuild(string guildName);
    SocketGuild? GetGuild(ulong guildId);
    SocketRole? GetRole(string guildName, string roleName);
    SocketRole? GetRole(string guildName, ulong roleID);
    SocketRole? GetRole(ulong guildId, string roleName);
    SocketRole? GetRole(ulong guildId, ulong roleID);
    IReadOnlyCollection<SocketRole> GetRoles(string guildName, UserId userId);
    IReadOnlyCollection<SocketRole> GetRoles(ulong guildId, UserId userId);
    IReadOnlyCollection<SocketGuildUser> GetUsers(string guildName);
    IReadOnlyCollection<SocketGuildUser> GetUsers(ulong guildId);
    Dictionary<SocketGuildUser, SocketRole[]> GetChatUsers(string guildName, ChatId chatId);
    Dictionary<SocketGuildUser, SocketRole[]> GetChatUsers(ulong guildId, ChatId chatId);

    Task SetStatusAsync(string status, CancellationToken cancellationToken = default);
}