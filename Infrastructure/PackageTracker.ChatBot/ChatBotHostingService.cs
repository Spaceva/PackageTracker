using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ChatBot;

public sealed class ChatBotHostingService<TChatBot>(TChatBot chatBot, ILogger<ChatBotHostingService<TChatBot>> logger) : BackgroundService
    where TChatBot : IChatBot
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var chatBotType = chatBot.BotName;
        try
        {
            logger.LogDebug("{BotType} started.", chatBotType);
            await chatBot.RunAsync(stoppingToken);
            logger.LogDebug("{BotType} ended.", chatBotType);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "{BotType} brutally stopped.", chatBotType);
        }
    }
}
