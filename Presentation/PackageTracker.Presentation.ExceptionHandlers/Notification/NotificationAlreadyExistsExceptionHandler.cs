using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Notifications.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Notification;
internal class NotificationAlreadyExistsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not NotificationAlreadyExistsException notificationAlreadyExistsException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(notificationAlreadyExistsException, StatusCodes.Status400BadRequest, cancellationToken: cancellationToken);
    }
}
