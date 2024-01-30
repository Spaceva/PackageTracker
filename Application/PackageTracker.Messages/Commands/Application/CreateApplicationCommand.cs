using MediatR;

namespace PackageTracker.Messages.Commands;

public class CreateApplicationCommand(ApplicationMessage applicationMessage) : ApplicationMessage(applicationMessage), IRequest
{
}
