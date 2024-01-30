using PackageTracker.Domain.Application.Model;
using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Package.Model;

public class ApplicationBranchNameComparer : IEqualityComparer<ApplicationBranch>, IComparer<ApplicationBranch>
{
    public int Compare(ApplicationBranch? x, ApplicationBranch? y)
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

    public bool Equals(ApplicationBranch? x, ApplicationBranch? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ApplicationBranch obj)
    {
        return obj.Name.GetHashCode();
    }
}
