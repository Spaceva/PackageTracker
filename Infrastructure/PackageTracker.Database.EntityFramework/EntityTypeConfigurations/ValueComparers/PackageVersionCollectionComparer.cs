using Microsoft.EntityFrameworkCore.ChangeTracking;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;
internal class PackageVersionCollectionComparer : ValueComparer<ICollection<PackageVersion>>
{
    private static bool CompareCollection(ICollection<PackageVersion>? c1, ICollection<PackageVersion>? c2)
    {
        var collection1 = c1 ?? [];
        var collection2 = c2 ?? [];
        return collection1.SequenceEqual(collection2, new PackageVersionComparer());
    }

    private static int Hashcode(ICollection<PackageVersion>? c)
    {
        var collection = c ?? [];
        return collection.Sum(c => c.GetHashCode());
    }

    public PackageVersionCollectionComparer()
        : base((c1, c2) => CompareCollection(c1, c2), c1 => Hashcode(c1))
    { }
}
