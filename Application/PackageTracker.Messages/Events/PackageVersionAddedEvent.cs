using MediatR;

namespace PackageTracker.Messages.Events;

public class PackageVersionAddedEvent : DestructuredPackageVersionMessage, INotification
{
}
