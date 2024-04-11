using PackageTracker.Host.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterConfigurations();

builder.RegisterServices();

builder.ConfigureLogging();

var application = builder.Build();

application.ConfigurePipeline(application.Environment);

application.ConfigureEndpoints();

application.ConfigureDatabase();

application.Run();