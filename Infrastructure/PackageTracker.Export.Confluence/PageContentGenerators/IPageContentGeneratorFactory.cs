namespace PackageTracker.Export.Confluence;

public interface IPageContentGeneratorFactory
{
    IPageContentGenerator CreatePageContentGenerator(string pageName, long pageId);
}
