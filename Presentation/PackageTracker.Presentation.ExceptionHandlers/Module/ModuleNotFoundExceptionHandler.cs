using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Presentation.ExceptionHandlers.Module;
internal class ModuleNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ModuleNotFoundException moduleNotFoundException)
        {
            return false;
        }

        return await httpContext.WriteProblemDetailsAsync(moduleNotFoundException, StatusCodes.Status404NotFound, cancellationToken: cancellationToken);
    }
}
