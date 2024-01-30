using System.Linq.Expressions;

namespace PackageTracker.Database.EntityFramework.Extensions;

internal static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>>? keySelector = null, int? skip = null, int? take = null)
    {
        if (keySelector is null || skip is null || take is null)
        {
            return query;
        }

        return query.OrderBy(keySelector).Skip(skip.Value).Take(take.Value);
    }
}
