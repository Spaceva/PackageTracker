using Discord;
using Discord.WebSocket;
using MediatR;
using PackageTracker.ChatBot.Discord;

namespace PackageTracker.ChatBot.Notifications.Discord;

public class DiscordNotifierBot(string token, IMediator mediator, IServiceProvider serviceProvider) : SingleGuildDiscordChatBot(mediator, serviceProvider)
{
    public override string Token => token;

    protected sealed override async Task PostStartAsync(CancellationToken cancellationToken) => await SetStatusAsync("Notifying...", cancellationToken);

    protected sealed override DiscordCommand ParseCommand(string command, string[] commandArgs, DiscordIncomingMessage incomingMessage) => new IgnoredCommand();

    protected override Task HandleGuildWebhooksUpdatedAsync(SocketGuild guild, SocketGuildChannel guildChannel) => Task.CompletedTask;

    protected override Task HandleGuildUserVoiceStateUpdatedAsync(SocketGuildUser guildUser, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateNow) => Task.CompletedTask;

    protected override Task HandleGuildUserIsTypingAsync(SocketGuildUser guildUser, SocketGuildChannel socketGuildChannel) => Task.CompletedTask;

    protected override Task HandleGuildUserUpdatedAsync(SocketUser userBefore, SocketGuildUser socketGuildUser) => Task.CompletedTask;

    protected override Task HandleGuildUserUnbannedAsync(SocketGuildUser socketGuildUser, SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleGuildChannelCreatedAsync(SocketGuildChannel socketGuildChannel) => Task.CompletedTask;

    protected override Task HandleGuildChannelUpdatedAsync(SocketChannel channelBefore, SocketGuildChannel socketGuildChannel) => Task.CompletedTask;

    protected override Task HandleGuildPresenceUpdatedAsync(SocketGuildUser socketGuildUser, SocketPresence presenceBefore, SocketPresence presenceNow) => Task.CompletedTask;

    protected override Task HandleThreadDeletedAsync(SocketThreadChannel threadChannel) => Task.CompletedTask;

    protected override Task HandleVoiceServerUpdatedAsync(SocketVoiceServer voiceServer) => Task.CompletedTask;

    protected override Task HandleUserIsTypingAsync(IUser user, IMessageChannel channel) => Task.CompletedTask;

    protected override Task HandleUserLeftAsync(SocketGuild guild, SocketUser user) => Task.CompletedTask;

    protected override Task HandleUserJoinedAsync(SocketGuildUser user) => Task.CompletedTask;

    protected override Task HandleUserCommandExecutedAsync(SocketUserCommand userCommand) => Task.CompletedTask;

    protected override Task HandleUserBannedAsync(SocketUser user, SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleThreadMemberLeftAsync(SocketThreadUser user) => Task.CompletedTask;

    protected override Task HandleThreadMemberJoinedAsync(SocketThreadUser user) => Task.CompletedTask;

    protected override Task HandleThreadCreatedAsync(SocketThreadChannel channel) => Task.CompletedTask;

    protected override Task HandleThreadUpdatedAsync(SocketThreadChannel threadChannelBefore, SocketThreadChannel threadChannelNow) => Task.CompletedTask;

    protected override Task HandleStageUpdatedAsync(SocketStageChannel channelBefore, SocketStageChannel channelNow) => Task.CompletedTask;

    protected override Task HandleStageStartedAsync(SocketStageChannel channel) => Task.CompletedTask;

    protected override Task HandleStageEndedAsync(SocketStageChannel channel) => Task.CompletedTask;

    protected override Task HandleSpeakerRemovedAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected override Task HandleSpeakerAddedAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected override Task HandleSlashCommandExecutedAsync(SocketSlashCommand slashCommand) => Task.CompletedTask;

    protected override Task HandleSelectMenuExecutedAsync(SocketMessageComponent messageComponent) => Task.CompletedTask;

    protected override Task HandleRoleUpdatedAsync(SocketRole roleBefore, SocketRole roleNow) => Task.CompletedTask;

    protected override Task HandleRoleDeletedAsync(SocketRole role) => Task.CompletedTask;

    protected override Task HandleRoleCreatedAsync(SocketRole role) => Task.CompletedTask;

    protected override Task HandleRequestToSpeakAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected override Task HandleRecipientRemovedAsync(SocketGroupUser user) => Task.CompletedTask;

    protected override Task HandleRecipientAddedAsync(SocketGroupUser user) => Task.CompletedTask;

    protected override Task HandleReactionsRemovedForEmoteAsync(IUserMessage userMessage, IMessageChannel channel, DiscordEmoji emote) => Task.CompletedTask;

    protected override Task HandleReactionsClearedAsync(IUserMessage userMessage, IMessageChannel channel) => Task.CompletedTask;

    protected override Task HandleReactionRemovedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user) => Task.CompletedTask;

    protected override Task HandleReactionAddedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user) => Task.CompletedTask;

    protected override Task HandleModalSubmittedAsync(SocketModal modal) => Task.CompletedTask;

    protected override Task HandleMessageUpdatedAsync(IMessage messageBefore, SocketMessage messageNow, ISocketMessageChannel channel) => Task.CompletedTask;

    protected override Task HandleMessagesBulkDeletedAsync(IReadOnlyCollection<IMessage> deletedMessages, IMessageChannel channel) => Task.CompletedTask;

    protected override Task HandleMessageDeletedAsync(IMessage deletedMessage, IMessageChannel channel) => Task.CompletedTask;

    protected override Task HandleMessageCommandExecutedAsync(SocketMessageCommand messageCommand) => Task.CompletedTask;

    protected override Task HandleLeftGuildAsync(SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleLatencyUpdatedAsync(int latencyBefore, int latencyNow) => Task.CompletedTask;

    protected override Task HandleJoinedGuildAsync(SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleInviteDeletedAsync(SocketGuildChannel channel, string url) => Task.CompletedTask;

    protected override Task HandleInviteCreatedAsync(SocketInvite invite) => Task.CompletedTask;

    protected override Task HandleInteractionCreatedAsync(SocketInteraction interation) => Task.CompletedTask;

    protected override Task HandleIntegrationUpdatedAsync(IIntegration integration) => Task.CompletedTask;

    protected override Task HandleIntegrationDeletedAsync(IGuild guild, ulong id, Optional<ulong> optionalId) => Task.CompletedTask;

    protected override Task HandleIntegrationCreatedAsync(IIntegration integration) => Task.CompletedTask;

    protected override Task HandleGuildUpdatedAsync(SocketGuild guildBefore, SocketGuild guildNow) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventUserRemoveAsync(IUser user, SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventUserAddAsync(IUser user, SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventUpdatedAsync(SocketGuildEvent guildEventBefore, SocketGuildEvent guildEventNow) => Task.CompletedTask;

    protected override Task HandleGuildMemberUpdatedAsync(SocketGuildUser user, SocketGuildUser guild) => Task.CompletedTask;

    protected override Task HandleGuildJoinRequestDeletedAsync(SocketGuildUser user, SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleGuildUnavailableAsync(SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleGuildStickerUpdatedAsync(SocketCustomSticker customerStickerBefore, SocketCustomSticker customerStickerNow) => Task.CompletedTask;

    protected override Task HandleGuildStickerDeletedAsync(SocketCustomSticker customerSticker) => Task.CompletedTask;

    protected override Task HandleGuildStickerCreatedAsync(SocketCustomSticker customerSticker) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventStartedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventCreatedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventCompletedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildScheduledEventCancelledAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected override Task HandleGuildMembersDownloadedAsync(SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleGuildAvailableAsync(SocketGuild guild) => Task.CompletedTask;

    protected override Task HandleBotUpdatedAsync(SocketSelfUser botBefore, SocketSelfUser botNow) => Task.CompletedTask;

    protected override Task HandleChannelDestroyedAsync(SocketChannel channel) => Task.CompletedTask;

    protected override Task HandleButtonExecutedAsync(SocketMessageComponent messageComponent) => Task.CompletedTask;

    protected override Task HandleAutocompleteExecutedAsync(SocketAutocompleteInteraction autocompleteInteraction) => Task.CompletedTask;

    protected override Task HandleApplicationCommandUpdatedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected override Task HandleApplicationCommandDeletedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected override Task HandleApplicationCommandCreatedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected override Task HandleEditMessageExceptionAsync(EditMessageFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleReactionMessageExceptionAsync(ReactionMessageFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
