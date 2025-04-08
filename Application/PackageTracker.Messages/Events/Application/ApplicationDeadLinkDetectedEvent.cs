using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationDeadLinkDetectedEvent : ApplicationMessage, INotification
{
    public ApplicationDeadLinkDetectedEvent(Application application)
    {
        Application = application;
    }
}
