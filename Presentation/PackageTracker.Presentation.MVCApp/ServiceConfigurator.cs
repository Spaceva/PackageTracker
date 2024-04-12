using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PackageTracker.Presentation.MVCApp;
public static class ServiceConfigurator
{
    public static void AddMvcAppServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services
            .AddControllersWithViews()
            .AddApplicationPart(assembly)
            .AddRazorRuntimeCompilation() 
            .AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.ConfigureOptions(typeof(UIConfigureOptions));
    }
}
