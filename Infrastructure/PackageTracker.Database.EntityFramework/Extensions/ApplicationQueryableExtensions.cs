using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework.Extensions;
internal static class ApplicationQueryableExtensions
{
    public static IQueryable<Application> ApplySearchCriteria(this IQueryable<Application> query, ApplicationSearchCriteria searchCriteria)
    {
        if (searchCriteria is null)
        {
            return query;
        }

        if (searchCriteria.RepositoryTypes is not null && searchCriteria.RepositoryTypes.Count > 0)
        {
            query = query.Where(a => searchCriteria.RepositoryTypes.Contains(a.RepositoryType));
        }

        if (searchCriteria.ApplicationName is not null && searchCriteria.ApplicationName.Length > 0)
        {
            query = query.Where(a => a.Name.Contains(searchCriteria.ApplicationName));
        }

        if (!searchCriteria.ShowSoonDecommissioned)
        {
            query = query.Where(app => !app.IsSoonDecommissioned);
        }

        if (!searchCriteria.ShowDeadLink)
        {
            query = query.Where(app => !app.IsDeadLink);
        }

        return query;
    }

    public static IQueryable<Application> FilterByApplicationTypes(this IQueryable<Application> query, IReadOnlyCollection<ApplicationType>? searchCriteria)
    {
        if (searchCriteria is null || searchCriteria.Count == 0)
        {
            return query;
        }

        var applicationTypesFilters = new List<Func<Application, bool>>();
        if (searchCriteria.Contains(ApplicationType.Angular))
        {
            applicationTypesFilters.Add(x => x is AngularApplication);
        }
        if (searchCriteria.Contains(ApplicationType.DotNet))
        {
            applicationTypesFilters.Add(x => x is DotNetApplication);
        }
        if (searchCriteria.Contains(ApplicationType.Php))
        {
            applicationTypesFilters.Add(x => x is PhpApplication);
        }
        if (searchCriteria.Contains(ApplicationType.Java))
        {
            applicationTypesFilters.Add(x => x is JavaApplication);
        }
        if (searchCriteria.Contains(ApplicationType.React))
        {
            applicationTypesFilters.Add(x => x is ReactApplication);
        }
        var applicationTypesFilter = applicationTypesFilters.CombineOr();

        return query.Where(app => applicationTypesFilter(app));
    }

    public static IQueryable<Application> FilterByLastCommitAfter(this IQueryable<Application> query, ApplicationSearchCriteria searchCriteria)
    {
        if (searchCriteria?.LastCommitAfter is null)
        {
            return query;
        }

        var predicate = (ApplicationBranch branch) => branch.LastCommit.HasValue && branch.LastCommit.Value > searchCriteria.LastCommitAfter.Value;
        if (searchCriteria.ApplyCommitFilterOnAllBranchs)
        {
            return query.Where(app => app.Branchs.All(predicate));

        }

        return query.Where(app => app.Branchs.Any(predicate));
    }

    public static IQueryable<Application> FilterByLastCommitBefore(this IQueryable<Application> query, ApplicationSearchCriteria searchCriteria)
    {
        if (searchCriteria?.LastCommitBefore is null)
        {
            return query;
        }

        var predicate = (ApplicationBranch branch) => branch.LastCommit.HasValue && branch.LastCommit.Value < searchCriteria.LastCommitBefore.Value;
        if (searchCriteria.ApplyCommitFilterOnAllBranchs)
        {
            return query.Where(app => app.Branchs.All(predicate));
        }

        return query.Where(app => app.Branchs.Any(predicate));
    }

    public static IQueryable<Application> FilterByFrameworkStatus(this IQueryable<Application> query, ApplicationSearchCriteria? searchCriteria)
    {
        if (searchCriteria is null || searchCriteria.FrameworkStatus is null)
        {
            return query;
        }

        var predicate = (ApplicationModule module) => module.Framework != null && module.Framework.Status == searchCriteria.FrameworkStatus.Value;
        if (searchCriteria.ApplyFrameworkStatusFilterOnAllModules)
        {
            return query.Where(app => app.Branchs.SelectMany(branch => branch.Modules).All(predicate));
        }

        return query.Where(app => app.Branchs.SelectMany(branch => branch.Modules).Any(predicate));
    }
}
