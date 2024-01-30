using PackageTracker.Domain.Package.Model;
using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Application.Model;
public class ApplicationPackageComparer : IEqualityComparer<ApplicationPackage>, IComparer<ApplicationPackage>
{
    public int Compare(ApplicationPackage? x, ApplicationPackage? y)
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

        if (!x!.PackageName.Equals(y!.PackageName, StringComparison.OrdinalIgnoreCase))
        {
            return x.PackageName.CompareTo(y.PackageName);
        }

        return new PackageVersionComparer().Compare(x.TrackedPackageVersion ?? new PackageVersion(x.PackageVersion), y.TrackedPackageVersion ?? new PackageVersion(y.PackageVersion));
    }

    public bool Equals(ApplicationPackage? x, ApplicationPackage? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ApplicationPackage obj)
    {
        return obj.PackageName.GetHashCode() + obj.PackageVersion.GetHashCode();
    }
}