namespace PackageTracker.ChatBot.Notifications.Configuration;

internal class NotificationSettings
{
    public required long ChatId { get; init; }
    public required ChatType Type { get; init; }
    public enum ChatType
    {
        User,
        Channel
    }
}
