using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Telegram.SDK.Base;
using PackageTracker.Telegram.SDK.Interfaces;
using System.Reflection;

namespace PackageTracker.Telegram;

public static class ServiceCollectionExtensions
{
    public static void AddTelegram(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITelegramChatBot, PackageTrackerTelegramBot>();

        services.AddHostedService<TelegramBotHostingService>();

        services.Configure<TelegramBotSettings>(configuration.GetSection("Telegram"));

        services.AddMediatR(Assembly.GetExecutingAssembly());
    }
}
