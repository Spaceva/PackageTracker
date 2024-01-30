using Dapplo.Confluence;
using Dapplo.Confluence.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PackageTracker.Export.Confluence.Core;
internal class ConfluenceClientWrapper
{
    private readonly IConfluenceClient confluenceClient;
    private readonly ILogger<ConfluenceClientWrapper> logger;

    public ConfluenceClientWrapper(IOptions<ConfluenceSettings> confluenceSettings, ILogger<ConfluenceClientWrapper> logger)
    {
        var confluenceSettingsValue = confluenceSettings.Value;

        confluenceClient = ConfluenceClient.Create(new Uri($"https://{confluenceSettingsValue.Domain}.atlassian.net/wiki"));
        confluenceClient.SetBasicAuthentication(confluenceSettingsValue.Username, confluenceSettingsValue.AccessToken);
        this.logger = logger;
    }

    public async Task UpdatePageAsync(long pageId, string newContent, CancellationToken cancellationToken)
    {
        try
        {
            var query = Where.And(Where.Type.IsPage, Where.Id.Is(pageId));
            var searchResult = await confluenceClient.Content.SearchAsync(query, cancellationToken: cancellationToken);
            var contentDigest = searchResult.Results.Single();
            var content = await confluenceClient.Content.GetAsync(contentDigest, ConfluenceClientConfig.ExpandGetContentForUpdate, cancellationToken);
            content.Body.Storage.Value = newContent;
            content.Version.Number++;
            await confluenceClient.Content.UpdateAsync(content, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while trying to update page {PageId}.", pageId);
            throw new Exceptions.ConfluenceException(exception);
        }
    }
}
