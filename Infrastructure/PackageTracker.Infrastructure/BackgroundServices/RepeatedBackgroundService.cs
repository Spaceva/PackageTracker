using Microsoft.Extensions.Logging;

namespace PackageTracker.Infrastructure.BackgroundServices;

public abstract class RepeatedBackgroundService(ILogger logger) : IScopedBackgroundService
{
    protected ILogger Logger => logger;

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
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
            Logger.LogInformation("Stopped");
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
