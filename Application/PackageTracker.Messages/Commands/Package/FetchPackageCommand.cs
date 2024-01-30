using MediatR;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages.Commands;

public class FetchPackageCommand(string packageName, PackageType packageType) : IRequest
{
    public string PackageName => packageName;
    public PackageType PackageType => packageType;
}
 