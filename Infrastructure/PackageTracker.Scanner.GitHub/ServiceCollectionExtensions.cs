using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Scanner.GitHub;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGitHub(this IServiceCollection services)
    {
        return services
            .AddKeyedScoped<IScannerRegistrator, GitHubUserScannerRegistrator>("GitHubUser")
            .AddKeyedScoped<IScannerRegistrator, GitHubOrganizationScannerRegistrator>("GitHubOrganization");
    }
}
