namespace PackageTracker.Export.Confluence;
public interface IConfluenceExportRegistrator
{
    IConfluenceExportRegistrator Register<TPageContentGeneratorFactory>() where TPageContentGeneratorFactory : class, IPageContentGeneratorFactory;
    IConfluenceExportRegistrator Register<TPageContentGeneratorFactory>(Func<IServiceProvider, TPageContentGeneratorFactory> factory) where TPageContentGeneratorFactory : class, IPageContentGeneratorFactory;
    IConfluenceExportRegistrator Register(Type type);
}
