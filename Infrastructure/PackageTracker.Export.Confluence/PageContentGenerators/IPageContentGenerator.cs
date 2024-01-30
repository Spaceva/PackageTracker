namespace PackageTracker.Export.Confluence;

public interface IPageContentGenerator
{
    Task<string> GenerateContentAsync(CancellationToken cancellationToken);
}