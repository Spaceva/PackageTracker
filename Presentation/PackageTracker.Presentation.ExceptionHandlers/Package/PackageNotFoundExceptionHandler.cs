using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Package.Exceptions;

namespace PackageTracker.Presentation.ExceptionHandlers.Package;
internal class PackageNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not PackageNotFoundException packageNotFoundException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(packageNotFoundException, StatusCodes.Status404NotFound, cancellationToken: cancellationToken);
    }
}
