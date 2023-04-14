using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PackageTracker.Handlers;
public static class ServiceCollectionExtensions
{
    public static void AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}
