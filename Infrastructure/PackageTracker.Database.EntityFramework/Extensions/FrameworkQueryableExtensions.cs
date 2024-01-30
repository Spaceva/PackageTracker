using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.EntityFramework.Extensions;
internal static class FrameworkQueryableExtensions
{
    public static IQueryable<Framework> ApplySearchCriteria(this IQueryable<Framework> query, FrameworkSearchCriteria? searchCriteria)
    {
        if (searchCriteria is null)
        {
            return query;
        }

        if (searchCriteria.Name is not null)
        {
            query = query.Where(f => f.Name.Contains(searchCriteria.Name));
        }

        if (searchCriteria.Version is not null)
        {
            query = query.Where(f => f.Version.Contains(searchCriteria.Version));
        }

        if (searchCriteria.Status is not null)
        {
            query = query.Where(f => searchCriteria.Status.Contains(f.Status));
        }

        if (searchCriteria.Channel is not null)
        {
            query = query.Where(f => searchCriteria.Channel.Contains(f.Channel));
        }

        if (searchCriteria.CodeName is not null)
        {
            query = query.Where(f => f.CodeName != null && f.CodeName.Contains(searchCriteria.CodeName));
        }

        if (searchCriteria.ReleaseDateMinimum is not null)
        {
            query = query.Where(f => f.ReleaseDate != null && f.ReleaseDate >= searchCriteria.ReleaseDateMinimum);
        }

        if (searchCriteria.ReleaseDateMaximum is not null)
        {
            query = query.Where(f => f.ReleaseDate != null && f.ReleaseDate <= searchCriteria.ReleaseDateMaximum);
        }

        if (searchCriteria.EndOfLifeMinimum is not null)
        {
            query = query.Where(f => f.EndOfLife != null && f.EndOfLife >= searchCriteria.EndOfLifeMinimum);
        }

        if (searchCriteria.EndOfLifeMaximum is not null)
        {
            query = query.Where(f => f.EndOfLife != null && f.EndOfLife <= searchCriteria.EndOfLifeMaximum);
        }

        return query;
    }
}
