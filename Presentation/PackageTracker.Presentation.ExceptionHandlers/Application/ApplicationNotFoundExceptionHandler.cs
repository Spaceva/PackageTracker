using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Application.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Application;
internal class ApplicationNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ApplicationNotFoundException applicationNotFoundException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(applicationNotFoundException, StatusCodes.Status404NotFound, cancellationToken: cancellationToken);
    }
}
