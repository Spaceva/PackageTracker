using PackageTracker.Domain.Framework.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class FrameworkMonitoredEvent(Framework framework) : INotification
{
    public Framework Framework => framework;
}
