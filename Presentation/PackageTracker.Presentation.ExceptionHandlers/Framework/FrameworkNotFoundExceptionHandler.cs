using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Framework.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Framework;
internal class FrameworkNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not FrameworkNotFoundException frameworkNotFoundException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(frameworkNotFoundException, StatusCodes.Status404NotFound, cancellationToken: cancellationToken);
    }
}
