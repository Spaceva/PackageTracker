using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.ApplicationModuleParsers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationModuleParsers(this IServiceCollection services)
    {
        services.AddScoped<IApplicationModuleParser<AngularModule>, AngularModuleParser>();

        services.AddScoped<IApplicationModuleParser<DotNetAssembly>, DotNetAssemblyParser>();
        services.AddScoped<IApplicationModuleParser<DotNetAssembly>, DotNetFrameworkAssemblyParser>();

        services.AddScoped<IApplicationModuleParser<PhpModule>, PhpModuleParser>();
        return services;
    }
}
