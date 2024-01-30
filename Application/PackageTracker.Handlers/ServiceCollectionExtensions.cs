using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PackageTracker.Handlers;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMainHandlers(this IServiceCollection services)
    => services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
}
