using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages.Queries;

public class GetPackagesQueryResponse
{
    public IReadOnlyCollection<Package> Packages { get; init; } = Array.Empty<Package>();
}
