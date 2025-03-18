namespace PackageTracker.Export.Confluence;
public interface IConfluenceExportRegistrator
{
    IConfluenceExportRegistrator Register<TPageContentGenerator>(long pageId, string pageName) where TPageContentGenerator : class, IPageContentGenerator;
    public IConfluenceExportRegistrator Register<TPageContentGenerator>(Func<IServiceProvider, object, TPageContentGenerator> factory, long pageId, string pageName) where TPageContentGenerator : class, IPageContentGenerator;
    IConfluenceExportRegistrator Register(Type type, long pageId, string pageName);
}
