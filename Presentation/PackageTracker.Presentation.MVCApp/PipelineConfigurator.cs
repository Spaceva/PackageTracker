using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PackageTracker.Presentation.MVCApp;
public static class PipelineConfigurator
{
    public static void ConfigureMVCAppPipeline(this IApplicationBuilder application, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            application.UseDeveloperExceptionPage();
        }
        else
        {
            application.UseExceptionHandler("/Home/Error");
            application.UseHsts();
        }

        application.UseStaticFiles();
    }
}
