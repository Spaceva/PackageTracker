using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Export.Confluence;
internal class ConfluenceExportRegistrator(IServiceCollection services) : IConfluenceExportRegistrator
{
    public IConfluenceExportRegistrator Register<TPageContentGeneratorFactory>()
         where TPageContentGeneratorFactory : class, IPageContentGeneratorFactory
    {
        services.AddScoped<IPageContentGeneratorFactory, TPageContentGeneratorFactory>();
        return this;
    }

    public IConfluenceExportRegistrator Register<TPageContentGeneratorFactory>(Func<IServiceProvider, TPageContentGeneratorFactory> factory)
         where TPageContentGeneratorFactory : class, IPageContentGeneratorFactory
    {
        services.AddScoped<IPageContentGeneratorFactory, TPageContentGeneratorFactory>(factory);
        return this;
    }

    public IConfluenceExportRegistrator Register(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.GetInterface(nameof(IPageContentGeneratorFactory)) is null)
        {
            throw new ArgumentException($"Type must implements {nameof(IPageContentGeneratorFactory)}");
        }

        services.AddScoped(typeof(IPageContentGeneratorFactory), type);
        return this;
    }
}
