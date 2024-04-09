using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Notifications.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Notification;
internal class NotificationNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotificationNotFoundException notificationNotFoundException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(notificationNotFoundException, StatusCodes.Status404NotFound, cancellationToken: cancellationToken);
    }
}
