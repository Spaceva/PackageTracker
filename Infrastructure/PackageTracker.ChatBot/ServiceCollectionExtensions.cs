using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.ChatBot;

public static class ServiceCollectionExtensions
{
    public static void AddChatBot<TChatBot>(this IServiceCollection services, Func<IServiceProvider, TChatBot>? factory = null)
        where TChatBot : class, IChatBot
    {
        if (factory is not null)
        {
            services.AddSingleton(factory);
        }
        else
        {
            services.AddSingleton<TChatBot>();
        }

        services.AddSingleton<IChatBot>(sp => sp.GetRequiredService<TChatBot>());
        services.AddHostedService<ChatBotHostingService<TChatBot>>();
    }
}
