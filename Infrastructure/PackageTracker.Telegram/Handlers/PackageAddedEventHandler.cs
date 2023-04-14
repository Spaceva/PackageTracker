using MediatR;
using Microsoft.Extensions.Options;
using PackageTracker.Messages.Events;
using PackageTracker.Telegram.SDK.Base;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.Handlers;

internal class PackageAddedEventHandler : INotificationHandler<PackageAddedEvent>
{
    private readonly ITelegramChatBot chatBot;
    private readonly string channelId;

    public PackageAddedEventHandler(ITelegramChatBot chatBot, IOptions<TelegramBotSettings> telegramBotSettings)
    {
        this.chatBot = chatBot;
        this.channelId= telegramBotSettings.Value.ChannelId;
    }

    public async Task Handle(PackageAddedEvent notification, CancellationToken cancellationToken)
    {
        var releaseTag = notification.NoReleasedVersion ? "[PRE-RELEASE]" : "[RELEASE]";
        await chatBot.SendTextMessageToUserAsync(channelId, $"{chatBot.BeginBoldTag}New Package {notification.Type}{chatBot.EndBoldTag}{Environment.NewLine}<a href=\"{notification.Link}\">{notification.Name}</a>{Environment.NewLine}v{notification.LatestVersionLabel} {releaseTag}");
    }
}
