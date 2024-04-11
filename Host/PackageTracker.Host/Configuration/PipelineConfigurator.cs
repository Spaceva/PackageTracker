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
        application.UseHttpsRedirection();
        application.UseExceptionHandler();
    }
}
