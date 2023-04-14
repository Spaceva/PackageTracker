using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PackageTracker.Infrastructure.BackgroundServices;

public abstract class RepeatedBackgroundService : BackgroundService
{
    protected ILogger Logger { get; }

    public RepeatedBackgroundService(ILogger logger)
    {
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
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
            Logger.LogInformation("{Name} being stopped.", GetType().Name);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("{Name} being stopped.", GetType().Name);
        }
        finally
        {
            await CloseServiceAsync();
        }
    }

    protected abstract Task CloseServiceAsync();

    protected abstract Task RunIterationAsync(CancellationToken stoppingToken);

    protected abstract Task HandleRunErrorAsync(Exception exception, CancellationToken stoppingToken);

    protected abstract TimeSpan TimeBetweenEachExecution();

    protected async Task WaitForNextExecution(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Paused for {TimeBetweenEachExecution}.", TimeBetweenEachExecution());
        await Task.Delay(TimeBetweenEachExecution(), stoppingToken);
        Logger.LogInformation("Restarting.");
    }
}
