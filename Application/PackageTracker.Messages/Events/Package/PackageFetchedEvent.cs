using PackageTracker.Domain.Package.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class PackageFetchedEvent : PackageMessage, INotification
{
    public PackageFetchedEvent(Package package)
    {
        Package = package;
    }
}
