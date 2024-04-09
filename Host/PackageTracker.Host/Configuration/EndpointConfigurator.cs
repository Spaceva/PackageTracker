using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Presentation.MVCApp;
using PackageTracker.Presentation.WebApi;

namespace PackageTracker.Host.Configuration;
internal static class EndpointConfigurator
{
    public static void ConfigureEndpoints(this IEndpointRouteBuilder application)
    {
        application.ConfigureControllerEndpoints();
        application.MapGroup("/api").ConfigureApiEndpoints();
        application.MapHealthChecks("/health").ShortCircuit();
    }
}
