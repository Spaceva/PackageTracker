using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Export.Confluence;
internal class ConfluenceExportRegistrator(IServiceCollection services) : IConfluenceExportRegistrator
{
    public IConfluenceExportRegistrator Register<TPageContentGenerator>(long pageId, string pageName)
         where TPageContentGenerator : class, IPageContentGenerator
    {
        services.AddKeyedScoped<IPageContentGenerator, TPageContentGenerator>($"{pageId}-{pageName}");
        return this;
    }

    public IConfluenceExportRegistrator Register<TPageContentGenerator>(Func<IServiceProvider, object, TPageContentGenerator> factory, long pageId, string pageName)
         where TPageContentGenerator : class, IPageContentGenerator
    {
        var key = $"{pageId}-{pageName}";
        services.AddKeyedScoped<IPageContentGenerator, TPageContentGenerator>(key, (sp, obj) => factory(sp, key));
        return this;
    }

    public IConfluenceExportRegistrator Register(Type type, long pageId, string pageName)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.GetInterface(nameof(IPageContentGenerator)) is null)
        {
            throw new ArgumentException($"Type must implements {nameof(IPageContentGenerator)}");
        }

        services.AddKeyedScoped(typeof(IPageContentGenerator), $"{pageId}-{pageName}", type);
        return this;
    }
}
