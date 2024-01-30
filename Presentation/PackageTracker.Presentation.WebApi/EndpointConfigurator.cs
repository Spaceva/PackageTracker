using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.WebHost.RateLimiters;

namespace PackageTracker.Presentation.WebApi;

public static class EndpointConfigurator
{
    public static RouteGroupBuilder ConfigureApiEndpoints(this RouteGroupBuilder route)
    {
        route.RequireRateLimiting(new ApiRateLimiter());
        route.CacheOutput(opt => opt.Expire(TimeSpan.FromSeconds(30)));

        route.MapGroup("/packages").MapPackagesApiEndpoints().WithTags("Packages");
        route.MapGroup("/frameworks").MapFrameworksApiEndpoints().WithTags("Frameworks");
        route.MapGroup("/applications").MapApplicationsApiEndpoints().WithTags("Applications");

        return route
            .WithOpenApi();
    }
}
