using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Domain.Package;
public class PackageSearchCriteria
{
    public string? Name { get; init; }

    public IReadOnlyCollection<PackageType>? Types { get; init; }
}
