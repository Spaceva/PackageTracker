using PackageTracker.ChatBot.Telegram;
using Telegram.Bot.Types;

namespace PackageTracker.ChatBot.Notifications.Telegram;
internal class TelegramNotifierBot(string token, IServiceProvider serviceProvider) : TelegramChatBot(serviceProvider)
{
    public override string Token => token;

    protected override TelegramCommand ParseCommand(string command, string[] commandArgs, TelegramIncomingMessage incomingMessage)
     => new IgnoredCommand();

    protected override Task HandleCallbackQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleChannelChatCreatedAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleChannelPostUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleChatJoinRequestUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleChatMemberUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleChosenInlineResultUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleEditedChannelPostAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleEditedMessageAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleEditMessageExceptionAsync(EditMessageFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleEventBotJoinedAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleEventBotLeavedAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleEventNewChatMemberAsync(TelegramIncomingMessage incomingMessage, User newChatMember) => Task.CompletedTask;

    protected override Task HandleEventNewChatTitleAsync(TelegramIncomingMessage incomingMessage, string newChatTitle) => Task.CompletedTask;

    protected override Task HandleEventUserLeavedAsync(TelegramIncomingMessage incomingMessage, UserId userID) => Task.CompletedTask;

    protected override Task HandleGroupChatCreatedAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleInlineQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleMessagePinnedAsync(TelegramIncomingMessage incomingMessage, UserId pinnerUserId, string content) => Task.CompletedTask;

    protected override Task HandleMessageTextUpdateAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleMigrationToSuperGroupAsync(TelegramIncomingMessage incomingMessage, ChatId oldChatID, ChatId newChatID) => Task.CompletedTask;

    protected override Task HandleMyChatMemberUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandlePollAnswerUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandlePollUpdateAsync(TelegramIncomingMessage telegramIncomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandlePreCheckoutQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleReactionMessageExceptionAsync(ReactionMessageFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleShippingQueryUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleSupergroupChatCreatedAsync(TelegramIncomingMessage incomingMessage) => Task.CompletedTask;

    protected override Task HandleUnknownUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleUpdateFailedAsync(TelegramIncomingMessage incomingMessage, Exception ex) => Task.CompletedTask;

    protected override Task ReactToMessageInternalAsync(ChatId chatId, MessageId messageId, IEmoji emoji, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleMessageTextUpdateAsync(TelegramIncomingMessage incomingMessage, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected override Task HandleUpdateFailedAsync(TelegramIncomingMessage incomingMessage, Exception ex, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
