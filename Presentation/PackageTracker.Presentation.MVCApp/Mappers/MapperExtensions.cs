using AutoMapper;

namespace PackageTracker.Presentation.MVCApp.Mappers;
internal static class MapperExtensions
{
    public static IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(this IMapper mapper, IReadOnlyCollection<TSource> collection)
    {
        return mapper.Map<IReadOnlyCollection<TSource>, IReadOnlyCollection<TDestination>>(collection);
    }
}
