using PackageTracker.Cache;
using PackageTracker.Fetcher;
using PackageTracker.Handlers;
using PackageTracker.Telegram;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

AddConfigurations(builder);

var application = builder.Build();

Configure(application);

application.Run();

static void Configure(WebApplication application)
{
    ConfigureMiddleware(application, application.Environment);
    ConfigureEndpoints(application);
}

static void AddConfigurations(WebApplicationBuilder builder)
{
    ConfigureConfiguration(builder.Configuration, builder.Environment);
    ConfigureServices(builder.Services, builder.Configuration, builder.Environment);
    ConfigureLogging(builder.Host);
}

static void ConfigureLogging(ConfigureHostBuilder host)
    => host.UseSerilog(ConfigureSerilog);

static void ConfigureSerilog(HostBuilderContext hostingContext, LoggerConfiguration loggerConfiguration)
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    Directory.CreateDirectory("App_Data/Logs");
    Serilog.Debugging.SelfLog.Enable(new StreamWriter("App_Data/Logs/serilog-failures.txt", true));
}

static void ConfigureConfiguration(IConfigurationBuilder configuration, IWebHostEnvironment environment)
 => configuration.AddJsonFile("logging.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"logging.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
{
    services.AddHandlers();

    services.AddControllersWithViews();

    services.AddHealthChecks();

    services.ConfigureFetcher(configuration);

    services.ConfigureFileCaches(configuration);

    services.AddTelegram(configuration);
}

static void ConfigureMiddleware(IApplicationBuilder application, IWebHostEnvironment environment)
{
    if (environment.IsDevelopment())
    {
        application.UseDeveloperExceptionPage();
    }
    else
    {
        application.UseExceptionHandler("/Home/Error");
        application.UseHsts();
    }

    application.UseHttpsRedirection();
    application.UseStaticFiles();
    application.UseRouting();
}

static void ConfigureEndpoints(IEndpointRouteBuilder application)
{
    application.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    application.MapHealthChecks("/health");
}