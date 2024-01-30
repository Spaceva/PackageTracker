using MediatR;

namespace PackageTracker.Messages.Commands;

public class CreatePackageCommand(PackageMessage packageMessage) : PackageMessage(packageMessage), IRequest
{
}
