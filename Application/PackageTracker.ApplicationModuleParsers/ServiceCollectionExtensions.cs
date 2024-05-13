using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Application;

namespace PackageTracker.ApplicationModuleParsers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationModuleParsers(this IServiceCollection services)
    {
        services.AddScoped<IApplicationModuleParser, AngularModuleParser>();
        services.AddScoped<IApplicationModuleParser, DotNetAssemblyParser>();
        services.AddScoped<IApplicationModuleParser, DotNetFrameworkAssemblyParser>();
        services.AddScoped<IApplicationModuleParser, PhpModuleParser>();
        services.AddScoped<IApplicationModuleParser, JavaModuleParser>();
        return services;
    }
}
