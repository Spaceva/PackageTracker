using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Telegram.SDK.Base;

namespace PackageTracker.Telegram;

internal class PackageTrackerTelegramBot : TelegramNotifierBot
{
    private readonly TelegramBotSettings telegramBotSettings;

    public PackageTrackerTelegramBot(IOptions<TelegramBotSettings> telegramBotSettings, ILogger<PackageTrackerTelegramBot> logger)
        : base(logger)
    {
        this.telegramBotSettings = telegramBotSettings.Value;
    }

    public override string Token => telegramBotSettings.Token;
}
