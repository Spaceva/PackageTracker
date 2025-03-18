using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.WebHost.RateLimiters;

namespace PackageTracker.Presentation.WebApi;

public static class EndpointConfigurator
{
    public static RouteGroupBuilder ConfigureApiEndpoints(this RouteGroupBuilder route)
    {
        route.RequireRateLimiting(new ApiRateLimiter())
            .CacheOutput(opt => opt.Expire(TimeSpan.FromSeconds(30)))
            .WithRequestTimeout(TimeSpan.FromSeconds(30));

        route.MapGroup("/packages").MapPackagesApiEndpoints();
        route.MapGroup("/frameworks").MapFrameworksApiEndpoints();
        route.MapGroup("/applications").MapApplicationsApiEndpoints();
        route.MapGroup("/modules").MapModulesApiEndpoints();

        return route
            .WithOpenApi();
    }
}
