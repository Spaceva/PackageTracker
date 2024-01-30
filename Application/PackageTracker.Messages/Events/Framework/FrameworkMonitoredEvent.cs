using MediatR;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Messages.Events;

public class FrameworkMonitoredEvent(Framework framework) : INotification
{
    public Framework Framework => framework;
}
