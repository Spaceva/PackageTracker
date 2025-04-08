using PackageTracker.Domain.Package.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class FetchPackagesCommand(IReadOnlyCollection<string> packagesName, PackageType packageType) : IRequest
{
    public IReadOnlyCollection<string> PackagesName => packagesName;
    public PackageType PackageType => packageType;
}
