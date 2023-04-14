using MediatR;

namespace PackageTracker.Messages.Commands;

public class CreatePackageCommand : DestructuredPackageMessage, IRequest
{
    public CreatePackageCommand(DestructuredPackageMessage packageMessage) : base(packageMessage) { }
}
