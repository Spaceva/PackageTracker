using PackageTracker.Presentation.MVCApp;

namespace PackageTracker.Host.Configuration;
internal static class PipelineConfigurator
{
    public static void ConfigurePipeline(this IApplicationBuilder application, IWebHostEnvironment environment)
    {
        application.ConfigureMVCAppPipeline(environment);

        application.UseSwagger();
        application.UseSwaggerUI();
        application.UseOutputCache();
        application.UseExceptionHandler();

        if (application is WebApplication webApplication && webApplication.Configuration.AsEnumerable().Any(kvp => kvp.Key.Contains("HTTPS")))
        {
            application.UseHttpsRedirection();
        }
    }
}
