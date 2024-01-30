using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Events;

public class ApplicationScannedEvent : ApplicationMessage, INotification
{
    public ApplicationScannedEvent(Application application)
    {
        Application = application;
    }
}
