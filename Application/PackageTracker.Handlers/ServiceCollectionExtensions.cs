using Microsoft.Extensions.DependencyInjection;
using PackageTracker.SharedKernel.Mediator;
using System.Reflection;

namespace PackageTracker.Handlers;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMainHandlers(this IServiceCollection services)
    => services.AddMediator(Assembly.GetExecutingAssembly());
}
