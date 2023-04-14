using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Base;

internal sealed class TelegramBotHostingService : BackgroundService
{
    private readonly ILogger<TelegramBotHostingService> logger;
    private readonly ITelegramChatBot chatBot;

    public TelegramBotHostingService(ITelegramChatBot chatBot, ILogger<TelegramBotHostingService> logger)
    {
        this.chatBot = chatBot;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await chatBot.RunAsync(stoppingToken);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "{BotName} has been stopped.", chatBot.BotName);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "{BotName} brutally stopped.", chatBot.BotName);
        }
    }
}
