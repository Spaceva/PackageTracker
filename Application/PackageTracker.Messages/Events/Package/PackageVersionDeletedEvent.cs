using MediatR;

namespace PackageTracker.Messages.Events;

public class PackageVersionDeletedEvent : PackageVersionMessage, INotification
{
}
