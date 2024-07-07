using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Scanner.AzureDevOps;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureDevOps(this IServiceCollection services)
    {
        return services.AddKeyedScoped<IScannerRegistrator, AzureDevOpsScannerRegistrator>("AzureDevOps");
    }
}