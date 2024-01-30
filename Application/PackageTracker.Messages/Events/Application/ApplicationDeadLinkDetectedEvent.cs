using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Events;

public class ApplicationDeadLinkDetectedEvent : ApplicationMessage, INotification
{
    public ApplicationDeadLinkDetectedEvent(Application application)
    {
        Application = application;
    }
}
