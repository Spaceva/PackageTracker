using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PackageTracker.Presentation.ExceptionHandlers.Technical;
internal class BadRequestHttpExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badHttpRequestException)
        {
            return false;
        }

        if (badHttpRequestException.InnerException is JsonException jsonException)
        {
            return await httpContext.WriteProblemDetailsAsync(jsonException, StatusCodes.Status400BadRequest, "JSON Body request is not properly formatted.", cancellationToken: cancellationToken);
        }

        return await httpContext.WriteProblemDetailsAsync(badHttpRequestException, StatusCodes.Status400BadRequest, "There was an issue with the parameters in the request.", cancellationToken: cancellationToken);
    }
}
