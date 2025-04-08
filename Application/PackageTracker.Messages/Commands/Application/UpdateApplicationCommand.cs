using PackageTracker.SharedKernel.Mediator;
namespace PackageTracker.Messages.Commands;

public class UpdateApplicationCommand : ApplicationMessage, IRequest
{
    public UpdateApplicationCommand() { }

    public UpdateApplicationCommand(ApplicationMessage applicationMessage) : base(applicationMessage) { }
}
