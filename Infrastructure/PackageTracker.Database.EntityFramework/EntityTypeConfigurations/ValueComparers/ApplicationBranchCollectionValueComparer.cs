using Microsoft.EntityFrameworkCore.ChangeTracking;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationBranchCollectionValueComparer : ValueComparer<ICollection<ApplicationBranch>>
{
    private static bool CompareCollection(ICollection<ApplicationBranch>? c1, ICollection<ApplicationBranch>? c2)
    {
        var collection1 = c1 ?? Array.Empty<ApplicationBranch>();
        var collection2 = c2 ?? Array.Empty<ApplicationBranch>();
        return collection1.SequenceEqual(collection2, new ApplicationBranchComparer());
    }

    private static int Hashcode(ICollection<ApplicationBranch>? c)
    {
        var collection = c ?? Array.Empty<ApplicationBranch>();
        return collection.Sum(c => c.GetHashCode());
    }

    public ApplicationBranchCollectionValueComparer()
        : base((c1, c2) => CompareCollection(c1, c2), c1 => Hashcode(c1))
    { }
}
