using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Package.Model;

public class PackageVersionComparer : IEqualityComparer<PackageVersion>, IComparer<PackageVersion>
{
    public int Compare(PackageVersion? x, PackageVersion? y)
    {
        if (x is null && y is null)
        {
            return 0;
        }

        if (x is null)
        {
            return -1;
        }

        if (y is null)
        {
            return 1;
        }

        if (x!.Major != y!.Major)
        {
            return x.Major.CompareTo(y.Major);
        }

        if (x.Minor != y!.Minor)
        {
            return x.Minor.CompareTo(y.Minor);
        }

        if (x.Patch != y!.Patch)
        {
            return x.Patch.CompareTo(y.Patch);
        }

        return 0;
    }

    public bool Equals(PackageVersion? x, PackageVersion? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] PackageVersion obj)
    {
        return obj.ToString().GetHashCode();
    }
}
