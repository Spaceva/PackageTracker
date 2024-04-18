using Discord;
using Discord.WebSocket;
using MediatR;

namespace PackageTracker.ChatBot.Discord;

public abstract class SingleGuildDiscordChatBot(IMediator mediator, IServiceProvider serviceProvider) : DiscordChatBot(mediator, serviceProvider)
{
    public IReadOnlyCollection<SocketRole> GetRoles(UserId userId) => Guild is not null ? GetRoles(Guild.Id, userId) : [];

    public Dictionary<SocketGuildUser, SocketRole[]>? GetChatUsers(ChatId chatId) => Guild is not null ? GetChatUsers(Guild.Id, chatId) : throw new Exception("No guild found.");

    protected SocketGuild? Guild
    {
        get
        {
            return DiscordClient!.Guilds?.SingleOrDefault();
        }
    }

    protected IReadOnlyCollection<SocketGuildUser>? GetUsers() => Guild is not null ? GetUsers(Guild.Id) : null;

    protected SocketGuildUser? GetUser(string nickname)
    {
        var users = GetUsers();
        return users?.SingleOrDefault(u => (u.Nickname ?? u.Username).Equals(nickname, StringComparison.InvariantCultureIgnoreCase));
    }

    protected SocketGuildUser? GetUser(ulong id)
    {
        var users = GetUsers();
        return users?.SingleOrDefault(u => u.Id == id);
    }

    protected SocketRole? GetRole(string roleName) => Guild is not null ? GetRole(Guild.Id, roleName) : null;

    protected SocketRole? GetRole(ulong roleID) => Guild is not null ? GetRole(Guild.Id, roleID) : null;

    protected SocketGuildChannel? GetChannel(string channelName) => Guild is not null ? GetChannel(Guild.Id, channelName) : null;

    protected SocketGuildChannel? GetChannel(ulong channelID) => Guild is not null ? GetChannel(Guild.Id, channelID) : null;

    protected override Task HandleWebhooksUpdatedAsync(SocketGuild guild, SocketChannel channel) => HandleGuildWebhooksUpdatedAsync(guild, guild.Channels.Single(c => c.Id.Equals(channel.Id)));

    protected override Task HandleUserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateNow) => HandleGuildUserVoiceStateUpdatedAsync(GuildUser(user), voiceStateBefore, voiceStateNow);

    protected override Task HandleUserUpdatedAsync(SocketUser userBefore, SocketUser userNow) => HandleGuildUserUpdatedAsync(userBefore, GuildUser(userNow));

    protected override Task HandleUserUnbannedAsync(SocketUser user, SocketGuild guild) => HandleGuildUserUnbannedAsync(GuildUser(user), guild);

    protected override Task HandlePresenceUpdatedAsync(SocketUser user, SocketPresence presenceBefore, SocketPresence presenceNow) => HandleGuildPresenceUpdatedAsync(GuildUser(user), presenceBefore, presenceNow);

    protected override Task HandleChannelUpdatedAsync(SocketChannel channelBefore, SocketChannel channelNow) => HandleGuildChannelUpdatedAsync(channelBefore, Guild!.Channels.Single(c => c.Id.Equals(channelNow.Id)));

    protected override Task HandleChannelCreatedAsync(SocketChannel channel) => HandleGuildChannelCreatedAsync(Guild!.Channels.Single(c => c.Id.Equals(channel.Id)));

    protected abstract Task HandleGuildWebhooksUpdatedAsync(SocketGuild guild, SocketGuildChannel guildChannel);

    protected abstract Task HandleGuildUserVoiceStateUpdatedAsync(SocketGuildUser guildUser, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateNow);

    protected abstract Task HandleGuildUserIsTypingAsync(SocketGuildUser guildUser, SocketGuildChannel socketGuildChannel);

    protected abstract Task HandleGuildUserUpdatedAsync(SocketUser userBefore, SocketGuildUser socketGuildUser);

    protected abstract Task HandleGuildUserUnbannedAsync(SocketGuildUser socketGuildUser, SocketGuild guild);

    protected abstract Task HandleGuildChannelCreatedAsync(SocketGuildChannel socketGuildChannel);

    protected abstract Task HandleGuildChannelUpdatedAsync(SocketChannel channelBefore, SocketGuildChannel socketGuildChannel);

    protected abstract Task HandleGuildPresenceUpdatedAsync(SocketGuildUser socketGuildUser, SocketPresence presenceBefore, SocketPresence presenceNow);

    protected SocketGuildUser GuildUser(IUser user) => Guild!.Users.Single(u => u.Id == user.Id);

    protected SocketGuildChannel GuildChannel(IChannel channel) => Guild!.Channels.Single(c => c.Id == channel.Id);
}
