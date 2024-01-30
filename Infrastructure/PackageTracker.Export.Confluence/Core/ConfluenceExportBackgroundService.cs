using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Export.Confluence.Core;
internal class ConfluenceExportBackgroundService(IPageContentGeneratorFactory pageContentGeneratorFactory,
    ConfluenceClientWrapper confluenceClientWrapper,
    IOptions<ConfluenceSettings> options,
    ILogger<ConfluenceExportBackgroundService> logger) : RepeatedBackgroundService(logger)
{
    protected override Task CloseServiceAsync()
    {
        return Task.CompletedTask;
    }

    protected override TimeSpan TimeBetweenEachExecution() => options.Value.TimeBetweenEachExecution;

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        foreach (var confluencePage in options.Value.Pages)
        {
            Logger.LogInformation("Updating page '{PageName}'", confluencePage.Name);
            try
            {
                var pageContentGenerator = pageContentGeneratorFactory.CreatePageContentGenerator(confluencePage.Name, confluencePage.Id);
                var newContent = await pageContentGenerator.GenerateContentAsync(stoppingToken);
                await confluenceClientWrapper.UpdatePageAsync(confluencePage.Id, newContent, stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error while trying to update page '{PageName}' (id = {PageId}).", confluencePage.Name, confluencePage.Id);
            }
        }
    }
}
