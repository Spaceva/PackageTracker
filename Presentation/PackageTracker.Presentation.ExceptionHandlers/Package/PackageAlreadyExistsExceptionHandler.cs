using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Package.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Package;
internal class PackageAlreadyExistsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not PackageAlreadyExistsException packageAlreadyExistsException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(packageAlreadyExistsException, StatusCodes.Status400BadRequest, cancellationToken: cancellationToken);
    }
}
