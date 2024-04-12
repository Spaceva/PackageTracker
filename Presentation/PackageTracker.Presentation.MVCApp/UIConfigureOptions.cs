using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace PackageTracker.Presentation.MVCApp;
internal class UIConfigureOptions(IWebHostEnvironment webHostEnvironment) : IPostConfigureOptions<StaticFileOptions>
{
    public void PostConfigure(string? name, StaticFileOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();
        if (options.FileProvider is null && webHostEnvironment.WebRootFileProvider is null)
        {
            throw new InvalidOperationException("Missing FileProvider.");
        }

        options.FileProvider ??= webHostEnvironment.WebRootFileProvider;

        var basePath = "wwwroot";

        var filesProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly(), basePath);
        options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
    }
}