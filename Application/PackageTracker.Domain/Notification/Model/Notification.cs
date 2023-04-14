namespace PackageTracker.Domain.Notifications.Model;

public class Notification
{
    public Guid Id { get; set; }

    public DateTime DateTime { get; set; }

    public string Content { get; set; } = default!;

    public string Type { get; set; } = default!;

    public bool IsRead { get; set; }
}
