using MediatR;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Discord.Notifications;

internal class PackageAddedEventHandler(IDiscordBot discordBot) : INotificationHandler<PackageAddedEvent>
{
    public async Task Handle(PackageAddedEvent notification, CancellationToken cancellationToken)
    => await discordBot.SendTextMessageToUserAsync(ChatIds.TESTUSER, $"New Package {discordBot.Bold(notification.Type.ToString())} : {discordBot.Bold(notification.Name)} - v{discordBot.Italic(notification.LatestVersionLabel ?? "")} {discordBot.Italic(notification.NoReleasedVersion ? "[PRE-RELEASE]" : "[RELEASE]")}.");
}
