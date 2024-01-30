using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

namespace PackageTracker.Presentation.MVCApp;
public static class ServiceConfigurator
{
    public static void AddMvcAppServices(this IServiceCollection services)
    {
        services.AddControllersWithViews().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}
