using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Application;
using System.Reflection;

namespace PackageTracker.ApplicationModuleParsers;

public static class ServiceCollectionExtensions
{
    public static readonly Type[] ApplicationModuleParsersTypes = [.. Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IApplicationModuleParser)))];

    public static IServiceCollection AddApplicationModuleParsers(this IServiceCollection services)
    {
        foreach (var applicationModuleParserType in ApplicationModuleParsersTypes)
        {
            services.AddScoped(typeof(IApplicationModuleParser), applicationModuleParserType);
        }
        return services;
    }
}
