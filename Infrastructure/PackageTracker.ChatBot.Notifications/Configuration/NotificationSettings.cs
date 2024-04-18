namespace PackageTracker.ChatBot.Notifications.Configuration;

internal class NotificationSettings
{
    public ulong ChatId { get; init; }
    public ChatType Type { get; init; }
    public enum ChatType
    {
        User,
        Channel
    }
}
