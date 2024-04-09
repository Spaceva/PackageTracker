using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PackageTracker.Presentation.ExceptionHandlers;
internal static class HttpContextExtensions
{
    public static async Task<bool> WriteProblemDetailsAsync(this HttpContext httpContext, Exception exception, int statusCode, IDictionary<string, object?>? extensions = null, CancellationToken cancellationToken = default)
    {
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Title = exception.Message,
            Status = statusCode,
            Type = exception.GetType().Name,
            Detail = exception.Message,
            Extensions = extensions ?? new Dictionary<string, object?>(),
        }, cancellationToken);

        return true;
    }
    public static async Task<bool> WriteProblemDetailsAsync(this HttpContext httpContext, Exception exception, int statusCode, string overrideTitle, IDictionary<string, object?>? extensions = null, CancellationToken cancellationToken = default)
    {
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Title = overrideTitle,
            Status = statusCode,
            Type = exception.GetType().Name,
            Detail = exception.Message,
            Extensions = extensions ?? new Dictionary<string, object?>(),
        }, cancellationToken);

        return true;
    }
}
