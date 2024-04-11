using Serilog;

namespace PackageTracker.Host.Configuration;
internal static class LoggingConfigurator
{
    public static void ConfigureLogging(this WebApplicationBuilder host)
    => host.Host.UseSerilog(ConfigureSerilog);
    
    private static void ConfigureSerilog(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
        try
        {
            Directory.CreateDirectory("logs");
            Serilog.Debugging.SelfLog.Enable(new StreamWriter("logs/serilog-failures.txt", true));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serilog selflog has been disabled because of a {ex.GetType().Name}: {ex.Message}");
            Serilog.Debugging.SelfLog.Disable();
        }
    }
}
