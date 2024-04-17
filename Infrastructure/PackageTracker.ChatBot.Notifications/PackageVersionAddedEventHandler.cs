using MediatR;
using PackageTracker.Messages.Events;

namespace PackageTracker.ChatBot.Discord.Notifications;

internal class PackageVersionAddedEventHandler(IDiscordBot discordBot) : INotificationHandler<PackageVersionAddedEvent>
{
    public async Task Handle(PackageVersionAddedEvent notification, CancellationToken cancellationToken)
     => await discordBot.SendTextMessageToUserAsync(ChatIds.TESTUSER, $"New Package Version for {discordBot.Bold(notification.PackageName)}: v{discordBot.Bold(notification.PackageVersionLabel)}.");
}
