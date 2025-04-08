using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class PackageVersionAddedEvent : PackageVersionMessage, INotification
{
}
