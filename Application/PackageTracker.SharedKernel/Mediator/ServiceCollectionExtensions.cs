using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace PackageTracker.SharedKernel.Mediator;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }

        services.TryAddScoped<IMediator, Mediator>();
        foreach (var assembly in assemblies)
        {
            var assemblyTypes = assembly.GetTypes().Where(t => !t.IsAbstract);
            var voidRequestHandlersByInterface = assemblyTypes.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));
            var responseRequestHandlers = assemblyTypes.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));
            var notificationHandlers = assemblyTypes.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));

            foreach (var handler in voidRequestHandlersByInterface)
            {
                Type[] interfaceTypes = [.. handler.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))];
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    var requestType = interfaceType.GetGenericArguments()[0];
                    if (requestType is null)
                    {
                        continue;
                    }

                    var handlerInterface = typeof(IRequestHandler<>).MakeGenericType(requestType);
                    services.AddScoped(handlerInterface, handler);
                }
            }

            foreach (var handler in responseRequestHandlers)
            {
                Type[] interfaceTypes = [.. handler.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))];
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    var requestType = interfaceType.GetGenericArguments()[0];
                    var responseType = interfaceType.GetGenericArguments()[1];
                    if (requestType is null || responseType is null)
                    {
                        continue;
                    }

                    var handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                    services.AddScoped(handlerInterface, handler);
                }
            }

            foreach (var handler in notificationHandlers)
            {
                Type[] interfaceTypes = [.. handler.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>))];
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }

                foreach (var interfaceType in interfaceTypes)
                {
                    var notificationType = interfaceType.GetGenericArguments()[0];
                    if (notificationType is null)
                    {
                        continue;
                    }

                    var handlerInterface = typeof(INotificationHandler<>).MakeGenericType(notificationType);
                    services.AddScoped(handlerInterface, handler);
                }
            }
        }

        return services;
    }
}
