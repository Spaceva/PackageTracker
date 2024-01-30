using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PackageTracker.Presentation.MVCApp;
public static class EndpointConfigurator
{
    public static void ConfigureControllerEndpoints(this IEndpointRouteBuilder application)
    {
        application.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
