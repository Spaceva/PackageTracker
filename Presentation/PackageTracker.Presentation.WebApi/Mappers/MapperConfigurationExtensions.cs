using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Presentation.WebApi.DTOs.Application;
using PackageTracker.Presentation.WebApi.DTOs.Framework;
using PackageTracker.Presentation.WebApi.DTOs.Package;

namespace PackageTracker.Presentation.WebApi.Mappers;
internal static class MapperConfigurationExtensions
{
    public static IMapperConfigurationExpression ConfigureDTOMappers(this IMapperConfigurationExpression mapperConfiguration)
    {
        mapperConfiguration.CreateMap<Application, ApplicationDto>()
            .AfterMap<ApplicationLinkMapperAction>();
        
        mapperConfiguration.CreateMap<ApplicationBranch, ApplicationBranchDto>();
        mapperConfiguration.CreateMap<ApplicationModule, ApplicationModuleDto>();
        mapperConfiguration.CreateMap<ApplicationPackage, ApplicationPackageDto>();

        mapperConfiguration.CreateMap<Framework, FrameworkDto>()
            .AfterMap<FrameworkLinkMapperAction>();

        mapperConfiguration.CreateMap<Package, PackageDto>()
            .AfterMap<PackageLinkMapperAction>();
        mapperConfiguration.CreateMap<Package, PackageWithVersionsDto>()
            .AfterMap<PackageLinkMapperAction>();
        mapperConfiguration.CreateMap<PackageVersion, PackageVersionDto>()
            .ForMember(dto => dto.Value, src => src.MapFrom(d => d.ToString()));

        return mapperConfiguration;
    }
    public static IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(this IMapper mapper, IReadOnlyCollection<TSource> collection)
    {
        return mapper.Map<IReadOnlyCollection<TSource>, IReadOnlyCollection<TDestination>>(collection);
    }

    public static void WithDisplayNameAndSummary(this IEndpointConventionBuilder endpointConventionBuilder, string name)
    {
        endpointConventionBuilder.WithDisplayName(name).WithSummary(name);
    }
}
