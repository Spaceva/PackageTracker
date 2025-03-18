using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Modules;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Infrastructure.Modules;

public abstract class ModuleBackgroundService(ILogger logger, IModuleManager moduleManager, TimeSpan? initialDelay = null) : IScopedBackgroundService
{
    protected ILogger Logger => logger;

    protected abstract string ModuleName { get; }

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        if (initialDelay is not null)
        {
            await Task.Delay(initialDelay.Value, stoppingToken);
        }

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var isModuleEnabled = await moduleManager.IsEnabledAsync(ModuleName, stoppingToken);
                    if (!isModuleEnabled)
                    {
                        Logger.LogInformation("Module {ModuleName} is disabled.", ModuleName);
                        await WaitForNextExecution(stoppingToken);
                        continue;
                    }

                    Logger.LogInformation("Starting...");
                    await RunIterationAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    await HandleRunErrorAsync(ex, stoppingToken);
                }

                await WaitForNextExecution(stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            Logger.LogInformation("Stopped.");
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Stopped.");
        }
        finally
        {
            await CloseServiceAsync();
        }
    }

    protected abstract Task CloseServiceAsync();

    protected abstract Task RunIterationAsync(CancellationToken stoppingToken);

    protected abstract TimeSpan TimeBetweenEachExecution();

    protected virtual Task HandleRunErrorAsync(Exception exception, CancellationToken stoppingToken)
    {
        Logger.LogError(exception, "An error occured");
        return Task.CompletedTask;
    }

    protected async Task WaitForNextExecution(CancellationToken stoppingToken)
    {
        var timeBetweenEachExecution = TimeBetweenEachExecution();
        Logger.LogInformation("Paused for {TimeBetweenEachExecution}.", timeBetweenEachExecution);
        await Task.Delay(timeBetweenEachExecution, stoppingToken);
        Logger.LogInformation("Restarting.");
    }
}
