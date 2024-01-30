using AutoMapper;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Presentation.MVCApp.Mappers;
using PackageTracker.Presentation.MVCApp.Models;

namespace PackageTracker.Presentation.MVCApp;
public static class MapperConfigurator
{
    public static IMapperConfigurationExpression ConfigureMVCAppMappers(this IMapperConfigurationExpression mapperConfiguration)
    {
        mapperConfiguration
            .CreateMap<Application, ApplicationDetailViewModel>()
            .ConvertUsing(app => app.ToApplicationDetailViewModel());

        mapperConfiguration
            .CreateMap<Package, PackageViewModel>()
            .ForMember(vm => vm.Type, o => o.MapFrom(src => src.Type.ToString()));

        mapperConfiguration
            .CreateMap<Package, PackageWithVersionsViewModel>()
            .ForMember(vm => vm.Type, o => o.MapFrom(src => src.Type.ToString()))
            .ForMember(vm => vm.Versions, o => o.MapFrom(src => src.VersionLabelsDescending().ToDictionary(p => p, CreateLink(src))));

        return mapperConfiguration;
    }

    private static Func<string, string> CreateLink(Package package)
    {
        return package.Type switch
        {
            PackageType.Npm => versionLabel => $"<a href='{package.Link}/v/{versionLabel}'>v{versionLabel}</a>",
            PackageType.Nuget => versionLabel => $"<a href='{package.Link}/{versionLabel}'>v{versionLabel}</a>",
            PackageType.Packagist => versionLabel => $"<a href='{package.Link}#{versionLabel}'>v{versionLabel}</a>",
            _ => versionLabel => versionLabel,
        };
    }
}
