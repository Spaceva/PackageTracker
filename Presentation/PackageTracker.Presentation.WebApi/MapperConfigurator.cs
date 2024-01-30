using AutoMapper;
using PackageTracker.Presentation.WebApi.Mappers;

namespace PackageTracker.Presentation.WebApi;
public static class MapperConfigurator
{
    public static IMapperConfigurationExpression ConfigureApiMappers(this IMapperConfigurationExpression mapperConfiguration)
    {
        return mapperConfiguration.ConfigureDTOMappers();
    }
}
