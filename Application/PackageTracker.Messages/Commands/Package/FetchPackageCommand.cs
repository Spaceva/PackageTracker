using PackageTracker.Domain.Package.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class FetchPackageCommand(string packageName, PackageType packageType) : IRequest
{
    public string PackageName => packageName;
    public PackageType PackageType => packageType;
}
 