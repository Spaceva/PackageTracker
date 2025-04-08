using PackageTracker.Domain.Framework.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class FrameworkAddedEvent(Framework framework) : INotification
{
    public Framework Framework => framework;
}
