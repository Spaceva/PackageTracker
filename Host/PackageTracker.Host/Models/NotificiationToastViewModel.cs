using PackageTracker.Domain.Notifications.Model;
using PackageTracker.Messages.Events;

namespace PackageTracker.WebHost.Models;

public class NotificiationToastViewModel
{
    public NotificiationToastViewModel(Notification notification)
    {
        Id = notification.Id.ToString();
        Message = notification.Content;
        Date = notification.DateTime.ToShortTimeString();
        Title = FormatTitle(notification.Type);
    }

    public string Title { get; init; } = default!;

    public string Message { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Id { get; init; } = default!;

    private string FormatTitle(string type)
    {
        return type switch
        {
            nameof(PackageAddedEvent) => "New Package",
            nameof(PackageVersionAddedEvent) => "New Package Version",
            _ => type,
        };
    }
}
