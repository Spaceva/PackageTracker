using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class PackageVersionDeletedEvent : PackageVersionMessage, INotification
{
}
