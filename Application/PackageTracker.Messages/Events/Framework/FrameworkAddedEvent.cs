using MediatR;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Messages.Events;

public class FrameworkAddedEvent(Framework framework) : INotification
{
    public Framework Framework => framework;
}
