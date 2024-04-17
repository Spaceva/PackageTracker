using Discord;
using Discord.WebSocket;
using MediatR;

namespace PackageTracker.ChatBot.Discord;

public class DiscordNotifierBot(string token, IMediator mediator, IServiceProvider serviceProvider) : DiscordChatBot(mediator, serviceProvider)
{
    public override string Token => token;

    protected sealed override async Task PostStartAsync(CancellationToken cancellationToken) => await SetStatusAsync("Notifying...");

    protected sealed override DiscordCommand ParseCommand(string command, string[] commandArgs, DiscordIncomingMessage incomingMessage) => new IgnoredCommand();

    protected sealed override Task HandleApplicationCommandCreatedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected sealed override Task HandleApplicationCommandDeletedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected sealed override Task HandleApplicationCommandUpdatedAsync(SocketApplicationCommand applicationCommand) => Task.CompletedTask;

    protected sealed override Task HandleAutocompleteExecutedAsync(SocketAutocompleteInteraction autocompleteInteraction) => Task.CompletedTask;

    protected sealed override Task HandleBotUpdatedAsync(SocketSelfUser botBefore, SocketSelfUser botNow) => Task.CompletedTask;

    protected sealed override Task HandleButtonExecutedAsync(SocketMessageComponent messageComponent) => Task.CompletedTask;

    protected sealed override Task HandleChannelCreatedAsync(SocketChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleChannelDestroyedAsync(SocketChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleChannelUpdatedAsync(SocketChannel channelBefore, SocketChannel channelNow) => Task.CompletedTask;

    protected sealed override Task HandleEditMessageExceptionAsync(EditMessageFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleGuildAvailableAsync(SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleGuildJoinRequestDeletedAsync(SocketGuildUser user, SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleGuildMembersDownloadedAsync(SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleGuildMemberUpdatedAsync(SocketGuildUser user, SocketGuildUser guild) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventCancelledAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventCompletedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventCreatedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventStartedAsync(SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventUpdatedAsync(SocketGuildEvent guildEventBefore, SocketGuildEvent guildEventNow) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventUserAddAsync(IUser user, SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildScheduledEventUserRemoveAsync(IUser user, SocketGuildEvent guildEvent) => Task.CompletedTask;

    protected sealed override Task HandleGuildStickerCreatedAsync(SocketCustomSticker customerSticker) => Task.CompletedTask;

    protected sealed override Task HandleGuildStickerDeletedAsync(SocketCustomSticker customerSticker) => Task.CompletedTask;

    protected sealed override Task HandleGuildStickerUpdatedAsync(SocketCustomSticker customerStickerBefore, SocketCustomSticker customerStickerNow) => Task.CompletedTask;

    protected sealed override Task HandleGuildUnavailableAsync(SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleGuildUpdatedAsync(SocketGuild guildBefore, SocketGuild guildNow) => Task.CompletedTask;

    protected sealed override Task HandleIntegrationCreatedAsync(IIntegration integration) => Task.CompletedTask;

    protected sealed override Task HandleIntegrationDeletedAsync(IGuild guild, ulong id, Optional<ulong> optionalId) => Task.CompletedTask;

    protected sealed override Task HandleIntegrationUpdatedAsync(IIntegration integration) => Task.CompletedTask;

    protected sealed override Task HandleInteractionCreatedAsync(SocketInteraction interation) => Task.CompletedTask;

    protected sealed override Task HandleInviteCreatedAsync(SocketInvite invite) => Task.CompletedTask;

    protected sealed override Task HandleInviteDeletedAsync(SocketGuildChannel channel, string url) => Task.CompletedTask;

    protected sealed override Task HandleJoinedGuildAsync(SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleLatencyUpdatedAsync(int latencyBefore, int latencyNow) => Task.CompletedTask;

    protected sealed override Task HandleLeftGuildAsync(SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleMessageCommandExecutedAsync(SocketMessageCommand messageCommand) => Task.CompletedTask;

    protected sealed override Task HandleMessageDeletedAsync(IMessage deletedMessage, IMessageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleMessagesBulkDeletedAsync(IReadOnlyCollection<IMessage> deletedMessages, IMessageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleMessageUpdatedAsync(IMessage messageBefore, SocketMessage messageNow, ISocketMessageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleModalSubmittedAsync(SocketModal modal) => Task.CompletedTask;

    protected sealed override Task HandlePresenceUpdatedAsync(SocketUser user, SocketPresence presenceBefore, SocketPresence presenceNow) => Task.CompletedTask;

    protected sealed override Task HandleReactionAddedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user) => Task.CompletedTask;

    protected sealed override Task HandleReactionRemovedAsync(IUserMessage userMessage, DiscordEmoji emote, IUser user) => Task.CompletedTask;

    protected sealed override Task HandleReactionsClearedAsync(IUserMessage userMessage, IMessageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleReactionMessageExceptionAsync(ReactionMessageFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleReactionsRemovedForEmoteAsync(IUserMessage userMessage, IMessageChannel channel, DiscordEmoji emote) => Task.CompletedTask;

    protected sealed override Task HandleRecipientAddedAsync(SocketGroupUser user) => Task.CompletedTask;

    protected sealed override Task HandleRecipientRemovedAsync(SocketGroupUser user) => Task.CompletedTask;

    protected sealed override Task HandleRequestToSpeakAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected sealed override Task HandleRoleCreatedAsync(SocketRole role) => Task.CompletedTask;

    protected sealed override Task HandleRoleDeletedAsync(SocketRole role) => Task.CompletedTask;

    protected sealed override Task HandleRoleUpdatedAsync(SocketRole roleBefore, SocketRole roleNow) => Task.CompletedTask;

    protected sealed override Task HandleSelectMenuExecutedAsync(SocketMessageComponent messageComponent) => Task.CompletedTask;

    protected sealed override Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex) => Task.CompletedTask;

    protected sealed override Task HandleSlashCommandExecutedAsync(SocketSlashCommand slashCommand) => Task.CompletedTask;

    protected sealed override Task HandleSpeakerAddedAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected sealed override Task HandleSpeakerRemovedAsync(SocketStageChannel channel, SocketGuildUser user) => Task.CompletedTask;

    protected sealed override Task HandleStageEndedAsync(SocketStageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleStageStartedAsync(SocketStageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleStageUpdatedAsync(SocketStageChannel channelBefore, SocketStageChannel channelNow) => Task.CompletedTask;

    protected sealed override Task HandleThreadCreatedAsync(SocketThreadChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleThreadDeletedAsync(SocketThreadChannel threadChannel) => Task.CompletedTask;

    protected sealed override Task HandleThreadMemberJoinedAsync(SocketThreadUser user) => Task.CompletedTask;

    protected sealed override Task HandleThreadMemberLeftAsync(SocketThreadUser user) => Task.CompletedTask;

    protected sealed override Task HandleThreadUpdatedAsync(SocketThreadChannel threadChannelBefore, SocketThreadChannel threadChannelNow) => Task.CompletedTask;

    protected sealed override Task HandleUserBannedAsync(SocketUser user, SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleUserCommandExecutedAsync(SocketUserCommand userCommand) => Task.CompletedTask;

    protected sealed override Task HandleUserIsTypingAsync(IUser user, IMessageChannel channel) => Task.CompletedTask;

    protected sealed override Task HandleUserJoinedAsync(SocketGuildUser user) => Task.CompletedTask;

    protected sealed override Task HandleUserLeftAsync(SocketGuild guild, SocketUser user) => Task.CompletedTask;

    protected sealed override Task HandleUserUnbannedAsync(SocketUser user, SocketGuild guild) => Task.CompletedTask;

    protected sealed override Task HandleUserUpdatedAsync(SocketUser userBefore, SocketUser userNow) => Task.CompletedTask;

    protected sealed override Task HandleUserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState voiceStateBefore, SocketVoiceState voiceStateNow) => Task.CompletedTask;

    protected sealed override Task HandleVoiceServerUpdatedAsync(SocketVoiceServer voiceServer) => Task.CompletedTask;

    protected sealed override Task HandleWebhooksUpdatedAsync(SocketGuild guild, SocketChannel channel) => Task.CompletedTask;
}
