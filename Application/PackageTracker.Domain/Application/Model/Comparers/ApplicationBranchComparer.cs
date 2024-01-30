using PackageTracker.Domain.Application.Model;
using System.Diagnostics.CodeAnalysis;

namespace PackageTracker.Domain.Package.Model;

public class ApplicationBranchComparer : IEqualityComparer<ApplicationBranch>, IComparer<ApplicationBranch>
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

        if (x!.Name != y!.Name)
        {
            return x.Name.CompareTo(y.Name);
        }

        if (x!.LastCommit != y!.LastCommit)
        {
            return x.LastCommit.GetValueOrDefault().CompareTo(y.LastCommit.GetValueOrDefault());
        }

        if (x.Modules.SequenceEqual(y.Modules, new ApplicationModuleComparer()))
        {
            return 0;
        }

        if (x.Modules.Count != y.Modules.Count)
        {
            return x.Modules.Count.CompareTo(y.Modules);
        }

        var xyModulesComparison = CompareModules(x.Modules, y.Modules, 1);
        if (xyModulesComparison != 0)
        {
            return xyModulesComparison;
        }
        
        var yxModulesComparison = CompareModules(y.Modules, x.Modules, -1);
        if (yxModulesComparison != 0)
        {
            return yxModulesComparison;
        }

        return 0;
    }

    public bool Equals(ApplicationBranch? x, ApplicationBranch? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ApplicationBranch obj)
    {
        return obj.Name.GetHashCode() + obj.Modules.Sum(m => m.GetHashCode());
    }

    private static int CompareModules(ICollection<ApplicationModule> xModules, ICollection<ApplicationModule> yModules, int valueIfOtherIsNull)
    {
        var moduleComparer = new ApplicationModuleComparer();
        foreach (var xModule in xModules)
        {
            var yModule = yModules.FirstOrDefault(m => m.Name.Equals(xModule.Name, StringComparison.OrdinalIgnoreCase));
            if (yModule is null)
            {
                return valueIfOtherIsNull;
            }

            var moduleComparison = moduleComparer.Compare(xModule, yModule);
            if (moduleComparison != 0)
            {
                return moduleComparison;
            }
        }

        return 0;
    }
}
