using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.ChatBot;

public static class ServiceCollectionExtensions
{
    public static void AddChatBot<TChatBot>(this IServiceCollection services)
        where TChatBot : class, IChatBot
    {
        services.AddSingleton<TChatBot>();
        services.AddSingleton<IChatBot>(sp => sp.GetRequiredService<TChatBot>());
        services.AddHostedService<ChatBotHostingService<TChatBot>>();
    }
    public static void AddChatBot<TChatBot>(this IServiceCollection services, Func<IServiceProvider, TChatBot> factory)
        where TChatBot : class, IChatBot
    {
        services.AddSingleton(factory);
        services.AddSingleton<IChatBot>(sp => sp.GetRequiredService<TChatBot>());
        services.AddHostedService<ChatBotHostingService<TChatBot>>();
    }
}
