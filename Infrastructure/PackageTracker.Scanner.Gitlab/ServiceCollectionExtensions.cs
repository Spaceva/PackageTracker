using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Scanner.Gitlab;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitlab(this IServiceCollection services)
    {
        return services.AddKeyedScoped<IScannerRegistrator, GitlabScannerRegistrator>("Gitlab");
    }
}
