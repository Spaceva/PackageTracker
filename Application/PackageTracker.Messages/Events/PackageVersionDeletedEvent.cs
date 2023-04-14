using MediatR;

namespace PackageTracker.Messages.Events;

public class PackageVersionDeletedEvent : DestructuredPackageVersionMessage, INotification
{
}
