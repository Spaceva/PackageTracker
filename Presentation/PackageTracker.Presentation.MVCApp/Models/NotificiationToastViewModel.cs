using PackageTracker.Domain.Notifications.Model;
using PackageTracker.Messages.Events;

namespace PackageTracker.Presentation.MVCApp.Models;

public class NotificiationToastViewModel : BasicToastViewModel
{
    public NotificiationToastViewModel(Notification notification)
    {
        Id = notification.Id.ToString();
        Message = notification.Content;
        Date = notification.DateTime.ToShortTimeString();
        Title = FormatTitle(notification.Type);
    }

    public string Date { get; init; }

    private string FormatTitle(string type)
    {
        return type switch
        {
            nameof(ApplicationAddedEvent) => "New Application",
            nameof(ApplicationBranchAddedEvent) => "New Branch on Application",
            nameof(ApplicationBranchCommittedEvent) => "New Commit on Branch",
            nameof(ApplicationBranchDeletedEvent) => "Branch removed from Application",
            nameof(ApplicationDeletedEvent) => "Application removed",
            nameof(ApplicationModuleAddedEvent) => "New Module in Application",
            nameof(ApplicationModuleDeletedEvent) => "Module removed from Application",
            nameof(ApplicationPackageVersionAddedEvent) => "New package on Application",
            nameof(ApplicationPackageVersionDeletedEvent) => "Package removed from Application",
            nameof(ApplicationPackageVersionUpdatedEvent) => "Package version updated on Application",
            nameof(ApplicationScannedEvent) => "Application detected",
            nameof(FrameworkAddedEvent) => "New Framework",
            nameof(FrameworkDeletedEvent) => "Framework removed",
            nameof(FrameworkMonitoredEvent) => "Framework detected",
            nameof(FrameworkUpdatedEvent) => "Framework updated",
            nameof(PackageAddedEvent) => "New Package",
            nameof(PackageDeletedEvent) => "Package removed",
            nameof(PackageFetchedEvent) => "Package detected",
            nameof(PackageVersionAddedEvent) => "New Package Version",
            nameof(PackageVersionDeletedEvent) => "Package Version removed",
            _ => type,
        };
    }
}
