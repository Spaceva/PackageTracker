using MediatR;
namespace PackageTracker.Messages.Commands;

public class UpdatePackageCommand(PackageMessage packageMessage) : PackageMessage(packageMessage), IRequest
{
}
