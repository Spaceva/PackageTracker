using MediatR;
using Microsoft.Extensions.Options;
using PackageTracker.Messages.Events;
using PackageTracker.Telegram.SDK.Base;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.Handlers;

internal class PackageVersionAddedEventHandler : INotificationHandler<PackageVersionAddedEvent>
{
    private readonly ITelegramChatBot chatBot;
    private readonly string channelId;

    public PackageVersionAddedEventHandler(ITelegramChatBot chatBot, IOptions<TelegramBotSettings> telegramBotSettings)
    {
        this.chatBot = chatBot;
        this.channelId = telegramBotSettings.Value.ChannelId;
    }

    public async Task Handle(PackageVersionAddedEvent notification, CancellationToken cancellationToken)
    {
        await chatBot.SendTextMessageToUserAsync(channelId, $"{chatBot.BeginBoldTag}New Package Version{chatBot.EndBoldTag}{Environment.NewLine}{chatBot.BeginBoldTag}<a href=\"{notification.PackageLink}\">{notification.PackageName}</a>{chatBot.EndBoldTag}{Environment.NewLine}v{notification.PackageVersionLabel}");
    }
}
