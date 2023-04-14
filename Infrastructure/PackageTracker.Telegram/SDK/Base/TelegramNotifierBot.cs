using Microsoft.Extensions.Logging;
using PackageTracker.Telegram.SDK.Convertibles;
using PackageTracker.Telegram.SDK.Exceptions;
using Telegram.Bot.Types;
using ChatId = PackageTracker.Telegram.SDK.Convertibles.ChatId;

namespace PackageTracker.Telegram.SDK.Base;
internal abstract class TelegramNotifierBot : TelegramChatBot
{
    public TelegramNotifierBot(ILogger logger)
        : base(logger)
    {
    }

    protected override Task HandleCallbackQueryUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleChannelChatCreatedAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleChannelPostAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleChosenInlineResultUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleCommandUpdateAsync(TelegramIncomingMessage update, string command, string[] commandArgs)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEditedChannelPostAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEditedMessageAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEditMessageExceptionAsync(EditMessageFailedException ex)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventBotJoinedAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventBotLeavedAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventNewChatMemberAsync(TelegramIncomingMessage update, User newChatMember)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventNewChatTitleAsync(TelegramIncomingMessage update, string newChatTitle)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleEventUserLeavedAsync(TelegramIncomingMessage update, UserId userID)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleGroupChatCreatedAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleInlineQueryUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleMessagePinnedAsync(TelegramIncomingMessage update, UserId pinnerUserId, string content)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleMessageTextUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleMigrationToSuperGroupAsync(TelegramIncomingMessage update, ChatId oldChatID, ChatId newChatID)
    {
        return Task.CompletedTask;
    }

    protected override Task HandlePreCheckoutQueryUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleSendingFileExceptionAsync(SendFileToChatFailedException ex)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleSendingFileExceptionAsync(SendFileToUserFailedException ex)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToChatFailedException ex)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleSendingMessageExceptionAsync(SendMessageToUserFailedException ex)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleShippingQueryUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleSupergroupChatCreatedAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleUnknownUpdateAsync(TelegramIncomingMessage update)
    {
        return Task.CompletedTask;
    }

    protected override Task HandleUpdateFailedAsync(TelegramIncomingMessage update, Exception ex)
    {
        return Task.CompletedTask;
    }
}
