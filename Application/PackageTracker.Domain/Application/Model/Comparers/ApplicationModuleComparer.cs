using PackageTracker.Domain.Application.Model;
using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Package.Model;
public class ApplicationModuleComparer : IEqualityComparer<ApplicationModule>, IComparer<ApplicationModule>
{
    public int Compare(ApplicationModule? x, ApplicationModule? y)
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

        if (x!.Name != y!.Name)
        {
            return x.Name.CompareTo(y.Name);
        }

        if (x.Packages.SequenceEqual(y.Packages, new ApplicationPackageComparer()))
        {
            return 0;
        }

        if (x.Packages.Count != y.Packages.Count)
        {
            return x.Packages.Count.CompareTo(y.Packages.Count);
        }

        var xyPackagesComparison = ComparePackages(x.Packages, y.Packages, 1);
        if (xyPackagesComparison != 0)
        {
            return xyPackagesComparison;
        }

        var yxPackagesComparison = ComparePackages(y.Packages, x.Packages, -1);
        if (yxPackagesComparison != 0)
        {
            return yxPackagesComparison;
        }

        return 0;
    }

    public bool Equals(ApplicationModule? x, ApplicationModule? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ApplicationModule obj)
    {
        return obj.Name.GetHashCode() + obj.Packages.Sum(m => m.GetHashCode());
    }

    private static int ComparePackages(ICollection<ApplicationPackage> xPackages, ICollection<ApplicationPackage> yPackages, int valueIfOtherIsNull)
    {
        var packageComparer = new ApplicationPackageComparer();
        foreach (var xPackage in xPackages)
        {
            var yPackage = yPackages.FirstOrDefault(m => m.PackageName.Equals(xPackage.PackageName, StringComparison.OrdinalIgnoreCase));
            if (yPackage is null)
            {
                return valueIfOtherIsNull;
            }

            var moduleComparison = packageComparer.Compare(xPackage, yPackage);
            if (moduleComparison != 0)
            {
                return moduleComparison;
            }
        }

        return 0;
    }
}