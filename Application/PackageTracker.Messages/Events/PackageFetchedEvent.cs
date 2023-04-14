using MediatR;

namespace PackageTracker.Messages.Events;

public class PackageFetchedEvent : DestructuredPackageMessage, INotification
{
    public PackageFetchedEvent() { }
    
    public PackageFetchedEvent(DestructuredPackageMessage packageMessage) : base(packageMessage) { }
}
