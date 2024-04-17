using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PackageTracker.ChatBot.Discord.Notifications;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordNotificationHandlers(this IServiceCollection services)
    => services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
}
