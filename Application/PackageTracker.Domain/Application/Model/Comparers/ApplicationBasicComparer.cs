using System.Diagnostics.CodeAnalysis;
using App = PackageTracker.Domain.Application.Model.Application;

namespace PackageTracker.Domain.Package.Model;

public class ApplicationBasicComparer : IEqualityComparer<App>, IComparer<App>
{
    public int Compare(App? x, App? y)
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

        return x.RepositoryLink.CompareTo(y.RepositoryLink);
    }

    public bool Equals(App? x, App? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] App obj)
    {
        return obj.Name.GetHashCode() + obj.Type.GetHashCode() + obj.RepositoryLink.GetHashCode();
    }
}
