namespace PackageTracker.Domain;

public static partial class EnumerableExtensions
{
    public static IOrderedEnumerable<TSource> OrderAscendingUsing<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => source.OrderBy(s => s, comparer);

    public static IOrderedEnumerable<TSource> OrderDescendingUsing<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) => source.OrderByDescending(s => s, comparer);

    public static IOrderedEnumerable<TSource> ThenAscending<TSource>(this IOrderedEnumerable<TSource> source, IComparer<TSource> comparer) => source.ThenBy(s => s, comparer);

    public static IOrderedEnumerable<TSource> ThenDescending<TSource>(this IOrderedEnumerable<TSource> source, IComparer<TSource> comparer) => source.ThenByDescending(s => s, comparer);
}
