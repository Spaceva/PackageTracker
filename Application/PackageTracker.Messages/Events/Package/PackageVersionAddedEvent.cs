using MediatR;

namespace PackageTracker.Messages.Events;

public class PackageVersionAddedEvent : PackageVersionMessage, INotification
{
}
