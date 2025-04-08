using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class CreateApplicationCommand(ApplicationMessage applicationMessage) : ApplicationMessage(applicationMessage), IRequest
{
}
