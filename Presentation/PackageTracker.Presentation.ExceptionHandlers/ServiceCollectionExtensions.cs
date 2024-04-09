using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Presentation.ExceptionHandlers.Application;
using PackageTracker.Presentation.ExceptionHandlers.Framework;
using PackageTracker.Presentation.ExceptionHandlers.Notification;
using PackageTracker.Presentation.ExceptionHandlers.Package;
using PackageTracker.Presentation.ExceptionHandlers.Technical;

namespace PackageTracker.Presentation.ExceptionHandlers;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<BadRequestHttpExceptionHandler>();

        services.AddExceptionHandler<ApplicationNotFoundExceptionHandler>();

        services.AddExceptionHandler<FrameworkNotFoundExceptionHandler>();

        services.AddExceptionHandler<PackageAlreadyExistsExceptionHandler>();
        services.AddExceptionHandler<PackageNotFoundExceptionHandler>();

        services.AddExceptionHandler<NotificationAlreadyExistsExceptionHandler>();
        services.AddExceptionHandler<NotificationNotFoundExceptionHandler>();

        return services;
    }
}
