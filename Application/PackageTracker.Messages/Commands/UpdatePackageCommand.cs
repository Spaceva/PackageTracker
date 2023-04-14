using MediatR;
using PackageTracker.Domain.Packages.Model;

namespace PackageTracker.Messages.Commands;

public class UpdatePackageCommand : IRequest
{
    public string Name { get; init; } = default!;

    public IReadOnlyCollection<PackageVersion> Versions { get; init; } = default!;
}
