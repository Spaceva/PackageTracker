using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Modules;
using PackageTracker.Infrastructure.Modules;

namespace PackageTracker.Export.Confluence.Core;
internal class ConfluenceExportBackgroundService(IServiceProvider serviceProvider,
    ConfluenceClientWrapper confluenceClientWrapper,
    IOptions<ConfluenceSettings> options,
    IModuleManager moduleManager,
    ILogger<ConfluenceExportBackgroundService> logger) : ModuleBackgroundService(logger, moduleManager, TimeSpan.FromSeconds(1))
{
    protected override string ModuleName => Constants.ModuleName;

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
                var pageContentGenerator = serviceProvider.GetRequiredKeyedService<IPageContentGenerator>($"{confluencePage.Id}-{confluencePage.Name}");
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
