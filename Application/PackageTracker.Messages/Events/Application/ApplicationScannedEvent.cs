using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationScannedEvent : ApplicationMessage, INotification
{
    public ApplicationScannedEvent(Application application)
    {
        Application = application;
    }
}
