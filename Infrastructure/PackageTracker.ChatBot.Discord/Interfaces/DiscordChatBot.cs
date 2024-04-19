using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ChatBot.Discord;

public abstract class DiscordChatBot(IServiceProvider serviceProvider) : ChatBot<SocketMessage, DiscordIncomingMessage, DiscordSendingMessageOptions, DiscordCommand>(serviceProvider), IDiscordBot
{
    private bool isReady = false;

    public override sealed string BeginBoldTag => "**";
    public override sealed string EndBoldTag => "**";
    public override sealed string BeginItalicTag => "*";
    public override sealed string EndItalicTag => "*";
    public override string CommandStarterChar => "?";

    public override sealed string BotName
    {
        get
        {
            if (DiscordClient.CurrentUser is null)
            {
                return base.BotName;
            }
            
            return DiscordClient.CurrentUser.Username;
        }
    }
    public override sealed UserId BotId
    {
        get
        {
            if (DiscordClient is null)
            {
                return default!;
            }

            return DiscordClient.CurrentUser.Id;
        }
    }

    public override bool IsReady => isReady;

    protected DiscordSocketClient DiscordClient { get; } = new(new DiscordSocketConfig { LogLevel = LogSeverity.Debug });

    private ManualResetEvent ReadyEvent { get; } = new ManualResetEvent(false);

    public SocketGuild? GetGuild(string guildName)
    {
        var guildId = DiscordClient.Guilds?.FirstOrDefault(g => g.Name.Equals(guildName, StringComparison.InvariantCultureIgnoreCase))?.Id;
        if (!guildId.HasValue)
        {
            return null;
        }

        return GetGuild(guildId.Value);
    }

    public SocketGuild? GetGuild(ulong guildId) => DiscordClient.GetGuild(guildId);

    public IReadOnlyCollection<SocketGuildUser> GetUsers(string guildName)
    {
        var guild = GetGuild(guildName);
        return guild?.Users ?? [];
    }

    public IReadOnlyCollection<SocketGuildUser> GetUsers(ulong guildId)
    {
        var guild = GetGuild(guildId);
        return guild?.Users ?? [];
    }

    public IReadOnlyCollection<SocketRole> GetRoles(string guildName, UserId userId)
    {
        var guild = GetGuild(guildName);
        var user = guild?.Users.SingleOrDefault(u => u.Id == userId);
        return user?.Roles ?? [];
    }

    public IReadOnlyCollection<SocketRole> GetRoles(ulong guildId, UserId userId)
    {
        var guild = GetGuild(guildId);
        var user = guild?.Users.SingleOrDefault(u => u.Id == userId);
        return user?.Roles ?? [];
    }

    public SocketRole? GetRole(string guildName, ulong roleID)
    {
        var guild = GetGuild(guildName);
        return guild?.Roles.SingleOrDefault(r => r.Id == roleID);
    }

    public SocketRole? GetRole(string guildName, string roleName)
    {
        var guild = GetGuild(guildName);
        return guild?.Roles.SingleOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
    }

    public SocketRole? GetRole(ulong guildId, ulong roleID)
    {
        var guild = GetGuild(guildId);
        return guild?.Roles.SingleOrDefault(r => r.Id == roleID);
    }

    public SocketRole? GetRole(ulong guildId, string roleName)
    {
        var guild = GetGuild(guildId);
        return guild?.Roles.SingleOrDefault(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
    }

    public SocketGuildChannel? GetChannel(string guildName, ulong channelID)
    {
        var guild = GetGuild(guildName);
        return guild?.Channels.SingleOrDefault(c => c.Id == channelID);
    }

    public SocketGuildChannel? GetChannel(string guildName, string channelName)
    {
        var guild = GetGuild(guildName);
        return guild?.Channels.SingleOrDefault(c => c.Name is not null && c.Name.Equals(channelName, StringComparison.CurrentCultureIgnoreCase));
    }

    public SocketGuildChannel? GetChannel(ulong guildId, ulong channelID)
    {
        var guild = GetGuild(guildId);
        return guild?.Channels.SingleOrDefault(c => c.Id == channelID);
    }

    public SocketGuildChannel? GetChannel(ulong guildId, string channelName)
    {
        var guild = GetGuild(guildId);
        return guild?.Channels.SingleOrDefault(c => c.Name is not null && c.Name.Equals(channelName, StringComparison.CurrentCultureIgnoreCase));
    }

    public Dictionary<SocketGuildUser, SocketRole[]> GetChatUsers(string guildName, ChatId chatId)
    {
        var guild = GetGuild(guildName);
        var chat = guild?.Channels.SingleOrDefault(c => c.Id == chatId);
        return chat is not null ? chat.Users.ToDictionary(u => u, u => u.Roles.ToArray()) : [];
    }

    public Dictionary<SocketGuildUser, SocketRole[]> GetChatUsers(ulong guildId, ChatId chatId)
    {
        var guild = GetGuild(guildId);
        var chat = guild?.Channels.SingleOrDefault(c => c.Id == chatId);
        return chat is not null ? chat.Users.ToDictionary(u => u, u => u.Roles.ToArray()) : [];
    }

    public async Task SetStatusAsync(string status, CancellationToken cancellationToken = default) => await DiscordClient.SetCustomStatusAsync(status);

    protected override sealed async Task StopAsync(CancellationToken cancellationToken)
    {
        isReady = false;
        await DiscordClient.SetStatusAsync(UserStatus.Offline);
        await DiscordClient.StopAsync();
        await DiscordClient.LogoutAsync();
        Logger.LogInformation("{BotName} stopped.", BotName);
    }

    protected override sealed async Task StartAsync(CancellationToken cancellationToken)
    {
        BindEvents();
        Logger.LogInformation("Bot init done.");
        await DiscordClient.LoginAsync(TokenType.Bot, token: Token);
        await DiscordClient.StartAsync();
        ReadyEvent.Reset();
        if (!ReadyEvent.WaitOne(TimeSpan.FromSeconds(30)))
        {
            throw new DiscordBotStartFailedException(BotName);
        }

        await DiscordClient.SetStatusAsync(UserStatus.Online);
        Logger.LogInformation("{BotName} started.", BotName);
        await PostStartAsync(cancellationToken);
    }

    protected virtual Task PostStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override sealed DiscordIncomingMessage ParseIncomingMessage(SocketMessage incomingMessage) => new (incomingMessage);

    protected override sealed async Task SimulateTypingInternalAsync(ChatId chatId, CancellationToken cancellationToken = default)
    {
        var channel = await GetChannelAsync(chatId);
        await channel.TriggerTypingAsync(DefaultRequestOptions(cancellationToken));
    }

    protected override sealed Task HandleEventUpdateInternalAsync(DiscordIncomingMessage incomingMessage, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Event {MessageId} handled.", incomingMessage.MessageId);
        return Task.CompletedTask;
    }

    protected override sealed async Task ReactToMessageInternalAsync(ChatId chatId, MessageId messageId, IEmoji emoji, CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await GetChannelAsync(chatId);
            var message = await channel.GetMessageAsync(messageId) as IUserMessage;
            await message!.AddReactionAsync(new DiscordEmoji(emoji.Emoji));
        }
        catch (Exception ex)
        {
            throw new ReactionMessageFailedException(ex.Message, messageId, chatId, ex, emoji);
        }
    }

    protected override sealed async Task SendTextMessageToChatInternalAsync(ChatId chatId, string message, DiscordSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await GetChannelAsync(chatId);
            await channel.SendMessageAsync(message, messageOptions is not null && messageOptions.IsTTS, messageOptions?.Embed, messageOptions?.RequestOptions ?? DefaultRequestOptions(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new SendMessageToChatFailedException(ex.Message, chatId, ex, messageOptions);
        }
    }

    protected override sealed async Task SendTextMessageToUserInternalAsync(UserId userId, string message, DiscordSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var dmChannel = await GetDMChannelAsync(userId, cancellationToken);
            await dmChannel.SendMessageAsync(message, messageOptions is not null && messageOptions.IsTTS, messageOptions?.Embed, messageOptions?.RequestOptions ?? DefaultRequestOptions(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new SendMessageToUserFailedException(ex.Message, userId, ex, messageOptions);
        }
    }

    protected override sealed async Task SendFileToChatInternalAsync(ChatId chatId, Stream dataStream, string fileName, string? message = null, DiscordSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await GetChannelAsync(chatId);
            await channel.SendFileAsync(dataStream, fileName, message, messageOptions is not null && messageOptions.IsTTS, messageOptions?.Embed, messageOptions?.RequestOptions ?? DefaultRequestOptions(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new SendFileToChatFailedException(ex.Message, chatId, dataStream, fileName, ex, messageOptions);
        }
    }

    protected override sealed async Task SendFileToUserInternalAsync(UserId userId, Stream dataStream, string fileName, string? message = null, DiscordSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var dmChannel = await GetDMChannelAsync(userId, cancellationToken);
            await dmChannel.SendFileAsync(dataStream, fileName, message, messageOptions is not null && messageOptions.IsTTS, messageOptions?.Embed, messageOptions?.RequestOptions ?? DefaultRequestOptions(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new SendFileToUserFailedException(ex.Message, userId, dataStream, fileName, ex, messageOptions);
        }
    }

    protected override sealed async Task EditMessageInternalAsync(ChatId chatId, MessageId messageId, string newMessageContent, DiscordSendingMessageOptions? messageOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await GetChannelAsync(chatId);
            var message = await channel.GetMessageAsync(messageId) as IUserMessage;
            await message!.ModifyAsync(m => m.Content = newMessageContent, messageOptions?.RequestOptions ?? DefaultRequestOptions(cancellationToken));
        }
        catch (Exception ex)
        {
            throw new EditMessageFailedException(ex.Message, messageId, chatId, ex, messageOptions);
        }
    }

    protected virtual Task HandleLogAsync(LogMessage log)
    {
        if (log.Exception is TaskCanceledException or OperationCanceledException)
        {
            return Task.CompletedTask;
        }

        LogLevel logLevel = log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information,
        };

        Logger.Log(logLevel, log.Exception, "[{Source}] - {Message}", log.Source, log.Message);
        return Task.CompletedTask;
    }

    protected virtual RequestOptions DefaultRequestOptions(CancellationToken cancellationToken = default) => new() { RetryMode = RetryMode.AlwaysFail, Timeout = TimeoutInSec * 1000, CancelToken = cancellationToken };

    protected abstract Task HandleThreadDeletedAsync(SocketThreadChannel threadChannel);

    protected abstract Task HandleWebhooksUpdatedAsync(SocketGuild guild, SocketChannel channel);

    protected abstract Task HandleVoiceServerUpdatedAsync(SocketVoiceServer voiceServer);

    protected abstract Task HandleUserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateNow);

    protected abstract Task HandleUserIsTypingAsync(IUser user, IMessageChannel channel);

    protected abstract Task HandleUserUpdatedAsync(SocketUser userBefore, SocketUser userNow);

    protected abstract Task HandleUserUnbannedAsync(SocketUser user, SocketGuild guild);

    protected abstract Task HandleUserLeftAsync(SocketGuild guild, SocketUser user);

    protected abstract Task HandleUserJoinedAsync(SocketGuildUser user);

    protected abstract Task HandleUserCommandExecutedAsync(SocketUserCommand userCommand);

    protected abstract Task HandleUserBannedAsync(SocketUser user, SocketGuild guild);

    protected abstract Task HandleThreadMemberLeftAsync(SocketThreadUser user);

    protected abstract Task HandleThreadMemberJoinedAsync(SocketThreadUser user);

    protected abstract Task HandleThreadCreatedAsync(SocketThreadChannel channel);

    protected abstract Task HandleThreadUpdatedAsync(SocketThreadChannel threadChannelBefore, SocketThreadChannel threadChannelNow);

    protected abstract Task HandleStageUpdatedAsync(SocketStageChannel channelBefore, SocketStageChannel channelNow);

    protected abstract Task HandleStageStartedAsync(SocketStageChannel channel);

    protected abstract Task HandleStageEndedAsync(SocketStageChannel channel);

    protected abstract Task HandleSpeakerRemovedAsync(SocketStageChannel channel, SocketGuildUser user);

    protected abstract Task HandleSpeakerAddedAsync(SocketStageChannel channel, SocketGuildUser user);

    protected abstract Task HandleSlashCommandExecutedAsync(SocketSlashCommand slashCommand);

    protected abstract Task HandleSelectMenuExecutedAsync(SocketMessageComponent messageComponent);

    protected abstract Task HandleRoleUpdatedAsync(SocketRole roleBefore, SocketRole roleNow);

    protected abstract Task HandleRoleDeletedAsync(SocketRole role);

    protected abstract Task HandleRoleCreatedAsync(SocketRole role);

    protected abstract Task HandleRequestToSpeakAsync(SocketStageChannel channel, SocketGuildUser user);

    protected abstract Task HandleRecipientRemovedAsync(SocketGroupUser user);

    protected abstract Task HandleRecipientAddedAsync(SocketGroupUser user);

    protected abstract Task HandleReactionsRemovedForEmoteAsync(IUserMessage userMessage, IMessageChannel channel, DiscordEmoji emote);

    protected abstract Task HandleReactionsClearedAsync(IUserMessage userMessage, IMessageChannel channel);

    protected abstract Task HandleReactionRemovedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user);

    protected abstract Task HandleReactionAddedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user);

    protected abstract Task HandlePresenceUpdatedAsync(SocketUser user, SocketPresence presenceBefore, SocketPresence presenceNow);

    protected abstract Task HandleModalSubmittedAsync(SocketModal modal);

    protected abstract Task HandleMessageUpdatedAsync(IMessage messageBefore, SocketMessage messageNow, ISocketMessageChannel channel);

    protected abstract Task HandleMessagesBulkDeletedAsync(IReadOnlyCollection<IMessage> deletedMessages, IMessageChannel channel);

    protected abstract Task HandleMessageDeletedAsync(IMessage deletedMessage, IMessageChannel channel);

    protected abstract Task HandleMessageCommandExecutedAsync(SocketMessageCommand messageCommand);

    protected abstract Task HandleLeftGuildAsync(SocketGuild guild);

    protected abstract Task HandleLatencyUpdatedAsync(int latencyBefore, int latencyNow);

    protected abstract Task HandleJoinedGuildAsync(SocketGuild guild);

    protected abstract Task HandleInviteDeletedAsync(SocketGuildChannel channel, string url);

    protected abstract Task HandleInviteCreatedAsync(SocketInvite invite);

    protected abstract Task HandleInteractionCreatedAsync(SocketInteraction interation);

    protected abstract Task HandleIntegrationUpdatedAsync(IIntegration integration);

    protected abstract Task HandleIntegrationDeletedAsync(IGuild guild, ulong id, Optional<ulong> optionalId);

    protected abstract Task HandleIntegrationCreatedAsync(IIntegration integration);

    protected abstract Task HandleGuildUpdatedAsync(SocketGuild guildBefore, SocketGuild guildNow);

    protected abstract Task HandleGuildScheduledEventUserRemoveAsync(IUser user, SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildScheduledEventUserAddAsync(IUser user, SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildScheduledEventUpdatedAsync(SocketGuildEvent guildEventBefore, SocketGuildEvent guildEventNow);

    protected abstract Task HandleGuildMemberUpdatedAsync(SocketGuildUser user, SocketGuildUser guild);

    protected abstract Task HandleGuildJoinRequestDeletedAsync(SocketGuildUser user, SocketGuild guild);

    protected abstract Task HandleGuildUnavailableAsync(SocketGuild guild);

    protected abstract Task HandleGuildStickerUpdatedAsync(SocketCustomSticker customerStickerBefore, SocketCustomSticker customerStickerNow);

    protected abstract Task HandleGuildStickerDeletedAsync(SocketCustomSticker customerSticker);

    protected abstract Task HandleGuildStickerCreatedAsync(SocketCustomSticker customerSticker);

    protected abstract Task HandleGuildScheduledEventStartedAsync(SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildScheduledEventCreatedAsync(SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildScheduledEventCompletedAsync(SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildScheduledEventCancelledAsync(SocketGuildEvent guildEvent);

    protected abstract Task HandleGuildMembersDownloadedAsync(SocketGuild guild);

    protected abstract Task HandleGuildAvailableAsync(SocketGuild guild);

    protected abstract Task HandleBotUpdatedAsync(SocketSelfUser botBefore, SocketSelfUser botNow);

    protected abstract Task HandleChannelUpdatedAsync(SocketChannel channelBefore, SocketChannel channelNow);

    protected abstract Task HandleChannelDestroyedAsync(SocketChannel channel);

    protected abstract Task HandleChannelCreatedAsync(SocketChannel channel);

    protected abstract Task HandleButtonExecutedAsync(SocketMessageComponent messageComponent);

    protected abstract Task HandleAutocompleteExecutedAsync(SocketAutocompleteInteraction autocompleteInteraction);

    protected abstract Task HandleApplicationCommandUpdatedAsync(SocketApplicationCommand applicationCommand);

    protected abstract Task HandleApplicationCommandDeletedAsync(SocketApplicationCommand applicationCommand);

    protected abstract Task HandleApplicationCommandCreatedAsync(SocketApplicationCommand applicationCommand);

    private void BindEvents()
    {
        DiscordClient.ApplicationCommandCreated += HandleApplicationCommandCreatedAsync;
        DiscordClient.ApplicationCommandDeleted += HandleApplicationCommandDeletedAsync;
        DiscordClient.ApplicationCommandUpdated += HandleApplicationCommandUpdatedAsync;
        DiscordClient.AutocompleteExecuted += HandleAutocompleteExecutedAsync;
        DiscordClient.ButtonExecuted += HandleButtonExecutedAsync;
        DiscordClient.ChannelCreated += HandleChannelCreatedAsync;
        DiscordClient.ChannelDestroyed += HandleChannelDestroyedAsync;
        DiscordClient.ChannelUpdated += HandleChannelUpdatedAsync;
        DiscordClient.Connected += HandleConnectedAsync;
        DiscordClient.CurrentUserUpdated += HandleBotUpdatedAsync;
        DiscordClient.Disconnected += HandleDisconnectedAsync;
        DiscordClient.GuildAvailable += HandleGuildAvailableAsync;
        DiscordClient.GuildJoinRequestDeleted += HandleGuildJoinRequestDeletedEventAsync;
        DiscordClient.GuildMembersDownloaded += HandleGuildMembersDownloadedAsync;
        DiscordClient.GuildMemberUpdated += HandleGuildMemberUpdatedEventAsync;
        DiscordClient.GuildScheduledEventCancelled += HandleGuildScheduledEventCancelledAsync;
        DiscordClient.GuildScheduledEventCompleted += HandleGuildScheduledEventCompletedAsync;
        DiscordClient.GuildScheduledEventCreated += HandleGuildScheduledEventCreatedAsync;
        DiscordClient.GuildScheduledEventStarted += HandleGuildScheduledEventStartedAsync;
        DiscordClient.GuildScheduledEventUpdated += HandleGuildScheduledEventUpdatedEventAsync;
        DiscordClient.GuildScheduledEventUserAdd += HandleGuildScheduledEventUserAddEventAsync;
        DiscordClient.GuildScheduledEventUserRemove += HandleGuildScheduledEventUserRemoveEventAsync;
        DiscordClient.GuildStickerCreated += HandleGuildStickerCreatedAsync;
        DiscordClient.GuildStickerDeleted += HandleGuildStickerDeletedAsync;
        DiscordClient.GuildStickerUpdated += HandleGuildStickerUpdatedAsync;
        DiscordClient.GuildUnavailable += HandleGuildUnavailableAsync;
        DiscordClient.GuildUpdated += HandleGuildUpdatedAsync;
        DiscordClient.IntegrationCreated += HandleIntegrationCreatedAsync;
        DiscordClient.IntegrationDeleted += HandleIntegrationDeletedAsync;
        DiscordClient.IntegrationUpdated += HandleIntegrationUpdatedAsync;
        DiscordClient.InteractionCreated += HandleInteractionCreatedAsync;
        DiscordClient.InviteCreated += HandleInviteCreatedAsync;
        DiscordClient.InviteDeleted += HandleInviteDeletedAsync;
        DiscordClient.JoinedGuild += HandleJoinedGuildAsync;
        DiscordClient.LatencyUpdated += HandleLatencyUpdatedAsync;
        DiscordClient.LeftGuild += HandleLeftGuildAsync;
        DiscordClient.Log += HandleLogAsync;
        DiscordClient.LoggedIn += HandleLoggedInAsync;
        DiscordClient.LoggedOut += HandleLoggedOutAsync;
        DiscordClient.MessageCommandExecuted += HandleMessageCommandExecutedAsync;
        DiscordClient.MessageDeleted += HandleMessageDeletedEventAsync;
        DiscordClient.MessageReceived += HandleMessageReceivedAsync;
        DiscordClient.MessagesBulkDeleted += HandleMessagesBulkDeletedEventAsync;
        DiscordClient.MessageUpdated += HandleMessageUpdatedEventAsync;
        DiscordClient.ModalSubmitted += HandleModalSubmittedAsync;
        DiscordClient.PresenceUpdated += HandlePresenceUpdatedAsync;
        DiscordClient.ReactionAdded += HandleReactionAddedEventAsync;
        DiscordClient.ReactionRemoved += HandleReactionRemovedEventAsync;
        DiscordClient.ReactionsCleared += HandleReactionsClearedEventAsync;
        DiscordClient.ReactionsRemovedForEmote += HandleReactionsRemovedForEmoteEventAsync;
        DiscordClient.Ready += HandleReadyAsync;
        DiscordClient.RecipientAdded += HandleRecipientAddedAsync;
        DiscordClient.RecipientRemoved += HandleRecipientRemovedAsync;
        DiscordClient.RequestToSpeak += HandleRequestToSpeakAsync;
        DiscordClient.RoleCreated += HandleRoleCreatedAsync;
        DiscordClient.RoleDeleted += HandleRoleDeletedAsync;
        DiscordClient.RoleUpdated += HandleRoleUpdatedAsync;
        DiscordClient.SelectMenuExecuted += HandleSelectMenuExecutedAsync;
        DiscordClient.SlashCommandExecuted += HandleSlashCommandExecutedAsync;
        DiscordClient.SpeakerAdded += HandleSpeakerAddedAsync;
        DiscordClient.SpeakerRemoved += HandleSpeakerRemovedAsync;
        DiscordClient.StageEnded += HandleStageEndedAsync;
        DiscordClient.StageStarted += HandleStageStartedAsync;
        DiscordClient.StageUpdated += HandleStageUpdatedAsync;
        DiscordClient.ThreadCreated += HandleThreadCreatedAsync;
        DiscordClient.ThreadDeleted += HandleThreadDeletedEventAsync;
        DiscordClient.ThreadMemberJoined += HandleThreadMemberJoinedAsync;
        DiscordClient.ThreadMemberLeft += HandleThreadMemberLeftAsync;
        DiscordClient.ThreadUpdated += HandleThreadUpdatedEventAsync;
        DiscordClient.UserBanned += HandleUserBannedAsync;
        DiscordClient.UserCommandExecuted += HandleUserCommandExecutedAsync;
        DiscordClient.UserIsTyping += HandleUserIsTypingEventAsync;
        DiscordClient.UserJoined += HandleUserJoinedAsync;
        DiscordClient.UserLeft += HandleUserLeftAsync;
        DiscordClient.UserUnbanned += HandleUserUnbannedAsync;
        DiscordClient.UserUpdated += HandleUserUpdatedAsync;
        DiscordClient.UserVoiceStateUpdated += HandleUserVoiceStateUpdatedAsync;
        DiscordClient.VoiceServerUpdated += HandleVoiceServerUpdatedAsync;
        DiscordClient.WebhooksUpdated += HandleWebhooksUpdatedAsync;
    }

    private static async Task<IEnumerable<IMessage>> DownloadAllMessagesAsync(IReadOnlyCollection<Cacheable<IMessage, ulong>> messagesCache)
    {
        var listMessages = new List<IMessage>();
        foreach (var item in messagesCache)
        {
            listMessages.Add(await item.GetOrDownloadAsync());
        }
        return listMessages;
    }

    private Task<ISocketMessageChannel> GetChannelAsync(ChatId chat) => Task.FromResult((ISocketMessageChannel)DiscordClient.GetChannel(chat));

    private async Task<IDMChannel> GetDMChannelAsync(UserId user, CancellationToken cancellationToken = default)
    {
        var userDiscordClient = await DiscordClient.GetUserAsync(user, DefaultRequestOptions(cancellationToken));
        return await userDiscordClient.CreateDMChannelAsync(DefaultRequestOptions(cancellationToken));
    }

    private Task HandleReadyAsync()
    {
        ReadyEvent.Set();
        Logger.LogInformation("{BotName} is {state}.", BotName, "ready");
        return Task.CompletedTask;
    }

    private Task HandleLoggedInAsync()
    {
        Logger.LogInformation("{BotName} is logged {state}.", BotName, "in");
        return Task.CompletedTask;
    }

    private Task HandleLoggedOutAsync()
    {
        ReadyEvent.Set();
        Logger.LogInformation("{BotName} is logged {state}.", BotName, "out");
        return Task.CompletedTask;
    }

    private async Task HandleDisconnectedAsync(Exception exception)
    {
        if (exception is TaskCanceledException or OperationCanceledException)
        {
            return;
        }

        Logger.LogError(exception, "{BotName} has been disconnected.", BotName);
        await Task.Delay(TimeSpan.FromSeconds(5));
        await DiscordClient.LoginAsync(TokenType.Bot, token: Token);
        await DiscordClient.StartAsync();
        ReadyEvent.Reset();
        if (!ReadyEvent.WaitOne(30 * 1000))
        {
            throw new DiscordBotStartFailedException(BotName);
        }

        await DiscordClient.SetStatusAsync(UserStatus.Online);
        Logger.LogInformation("{BotName} has been reconnected.", BotName);
    }

    private Task HandleConnectedAsync()
    {
        Logger.LogInformation("{BotName} is {state}.", BotName, "connected");
        isReady = true;
        return Task.CompletedTask;
    }

    private async Task HandleMessageReceivedAsync(SocketMessage message)
    {
        Logger.LogDebug("Received message {MessageId} from {UserId}.", message.Id, message.Author.Id);
        await HandleUpdateAsync(ParseIncomingMessage(message));
    }

    private async Task HandleUserIsTypingEventAsync(Cacheable<IUser, ulong> userCache, Cacheable<IMessageChannel, ulong> channelCache)
    {
        try
        {
            var user = await userCache.DownloadAsync();
            var channel = await channelCache.DownloadAsync();
            await HandleUserIsTypingAsync(user, channel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleUserIsTypingEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleUserIsTypingEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleReactionsRemovedForEmoteEventAsync(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, IEmote emote)
    {
        try
        {
            var userMessage = await messageCache.GetOrDownloadAsync();
            var channel = await channelCache.GetOrDownloadAsync();
            await HandleReactionsRemovedForEmoteAsync(userMessage, channel, new DiscordEmoji(emote.Name));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleReactionsRemovedForEmoteEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleReactionsRemovedForEmoteEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleReactionRemovedEventAsync(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
    {
        try
        {
            var emote = reaction.Emote;
            var userMessage = await messageCache.GetOrDownloadAsync();
            var emotingUser = reaction.User.GetValueOrDefault();
            await HandleReactionRemovedAsync(userMessage, new DiscordEmoji(emote.Name), emotingUser);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleReactionRemovedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleReactionRemovedAsync), ex.Message, ex));
        }
    }

    private async Task HandleReactionAddedEventAsync(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache, SocketReaction reaction)
    {
        try
        {
            var emote = reaction.Emote;
            var userMessage = await messageCache.GetOrDownloadAsync();
            var emotingUser = reaction.User.GetValueOrDefault();
            await HandleReactionAddedAsync(userMessage, new DiscordEmoji(emote.Name), emotingUser);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleReactionAddedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleReactionAddedAsync), ex.Message, ex));
        }
    }

    private async Task HandleReactionsClearedEventAsync(Cacheable<IUserMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache)
    {
        try
        {
            var userMessage = await messageCache.GetOrDownloadAsync();
            var channel = await channelCache.GetOrDownloadAsync();
            await HandleReactionsClearedAsync(userMessage, channel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleReactionsClearedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleReactionsClearedAsync), ex.Message, ex));
        }
    }

    private async Task HandleMessageUpdatedEventAsync(Cacheable<IMessage, ulong> messageCache, SocketMessage messageAfter, ISocketMessageChannel channel)
    {
        try
        {
            var userMessage = await messageCache.GetOrDownloadAsync();
            await HandleMessageUpdatedAsync(userMessage, messageAfter, channel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleMessageUpdatedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleMessageUpdatedAsync), ex.Message, ex));
        }
    }

    private async Task HandleMessagesBulkDeletedEventAsync(IReadOnlyCollection<Cacheable<IMessage, ulong>> messagesCache, Cacheable<IMessageChannel, ulong> channelCache)
    {
        try
        {
            var userMessages = await DownloadAllMessagesAsync(messagesCache);
            var channel = await channelCache.GetOrDownloadAsync();
            await HandleMessagesBulkDeletedAsync(userMessages.ToArray(), channel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleMessagesBulkDeletedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleMessagesBulkDeletedAsync), ex.Message, ex));
        }
    }

    private async Task HandleMessageDeletedEventAsync(Cacheable<IMessage, ulong> messageCache, Cacheable<IMessageChannel, ulong> channelCache)
    {
        try
        {
            var userMessage = await messageCache.GetOrDownloadAsync();
            var channel = await channelCache.GetOrDownloadAsync();
            await HandleMessageDeletedAsync(userMessage, channel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleMessageDeletedAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleMessageDeletedAsync), ex.Message, ex));
        }
    }

    private async Task HandleThreadUpdatedEventAsync(Cacheable<SocketThreadChannel, ulong> threadChannelCache, SocketThreadChannel threadChannelNow)
    {
        try
        {
            var threadChannelBefore = await threadChannelCache.GetOrDownloadAsync();
            await HandleThreadUpdatedAsync(threadChannelBefore, threadChannelNow);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleThreadUpdatedEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleThreadUpdatedEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleThreadDeletedEventAsync(Cacheable<SocketThreadChannel, ulong> threadChannelCache)
    {
        try
        {
            var threadChannel = await threadChannelCache.GetOrDownloadAsync();
            await HandleThreadDeletedAsync(threadChannel);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleThreadDeletedEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleThreadDeletedEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleGuildScheduledEventUserRemoveEventAsync(Cacheable<SocketUser, global::Discord.Rest.RestUser, IUser, ulong> userCache, SocketGuildEvent guildEvent)
    {
        try
        {
            var user = await userCache.GetOrDownloadAsync();
            await HandleGuildScheduledEventUserRemoveAsync(user, guildEvent);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleGuildScheduledEventUserRemoveEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleGuildScheduledEventUserRemoveEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleGuildScheduledEventUserAddEventAsync(Cacheable<SocketUser, global::Discord.Rest.RestUser, IUser, ulong> userCache, SocketGuildEvent guildEvent)
    {
        try
        {
            var user = await userCache.GetOrDownloadAsync();
            await HandleGuildScheduledEventUserAddAsync(user, guildEvent);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleGuildScheduledEventUserAddEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleGuildScheduledEventUserAddEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleGuildScheduledEventUpdatedEventAsync(Cacheable<SocketGuildEvent, ulong> guildEventCache, SocketGuildEvent guildEventNow)
    {
        try
        {
            var guildEventBefore = await guildEventCache.GetOrDownloadAsync();
            await HandleGuildScheduledEventUpdatedAsync(guildEventBefore, guildEventNow);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleGuildScheduledEventUpdatedEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleGuildScheduledEventUpdatedEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleGuildMemberUpdatedEventAsync(Cacheable<SocketGuildUser, ulong> userCache, SocketGuildUser guild)
    {
        try
        {
            var user = await userCache.GetOrDownloadAsync();
            await HandleGuildMemberUpdatedAsync(user, guild);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleGuildMemberUpdatedEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleGuildMemberUpdatedEventAsync), ex.Message, ex));
        }
    }

    private async Task HandleGuildJoinRequestDeletedEventAsync(Cacheable<SocketGuildUser, ulong> userCache, SocketGuild guild)
    {
        try
        {
            var user = await userCache.GetOrDownloadAsync();
            await HandleGuildJoinRequestDeletedAsync(user, guild);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(HandleGuildJoinRequestDeletedEventAsync)} failed.");
            await HandleLogAsync(new LogMessage(LogSeverity.Error, nameof(HandleGuildJoinRequestDeletedEventAsync), ex.Message, ex));
        }
    }
}
