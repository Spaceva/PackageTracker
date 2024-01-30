using PackageTracker.Domain.Application.Model;
using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Package.Model;
public class ApplicationModuleNameComparer : IEqualityComparer<ApplicationModule>, IComparer<ApplicationModule>
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

        return x!.Name.CompareTo(y!.Name);
    }

    public bool Equals(ApplicationModule? x, ApplicationModule? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ApplicationModule obj)
    {
        return obj.Name.GetHashCode();
    }
}