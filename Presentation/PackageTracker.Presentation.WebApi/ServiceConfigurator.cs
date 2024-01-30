using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Presentation.WebApi.Mappers;
using System.Text.Json.Serialization;

namespace PackageTracker.Presentation.WebApi;
public static class ServiceConfigurator
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services.AddScoped<ApplicationLinkMapperAction>()
            .AddScoped<FrameworkLinkMapperAction>()
            .AddScoped<PackageLinkMapperAction>();
    }
}
