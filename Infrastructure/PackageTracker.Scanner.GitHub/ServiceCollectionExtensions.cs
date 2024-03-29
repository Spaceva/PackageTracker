﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Scanner.GitHub;

public static class ServiceCollectionExtensions
{
    public static IScannerRegistrator AddAngularGitHubUserScanner(this IScannerRegistrator services, string trackerName)
        => AddAngularGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForUser(name));
    public static IScannerRegistrator AddAngularGitHubOrganizationScanner(this IScannerRegistrator services, string trackerName)
        => AddAngularGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForOrg(name));
    public static IScannerRegistrator AddDotNetGitHubUserScanner(this IScannerRegistrator services, string trackerName)
        => AddDotNetGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForUser(name));
    public static IScannerRegistrator AddDotNetGitHubOrganizationScanner(this IScannerRegistrator services, string trackerName)
        => AddDotNetGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForOrg(name));
    public static IScannerRegistrator AddPhpGitHubUserScanner(this IScannerRegistrator services, string trackerName)
        => AddPhpGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForUser(name));
    public static IScannerRegistrator AddPhpGitHubOrganizationScanner(this IScannerRegistrator services, string trackerName)
        => AddPhpGitHubScanner(services, trackerName, (gitHubClient, name) => gitHubClient.Repository.GetAllForOrg(name));

    private static IScannerRegistrator AddAngularGitHubScanner(this IScannerRegistrator services, string trackerName, Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<AngularModule>>();
            return new AngularGitHubScanner(getRepositoriesDelegate, trackedApplication, mediator, parsers, loggerFactory.CreateLogger<AngularGitHubScanner>());
        });

    private static IScannerRegistrator AddDotNetGitHubScanner(this IScannerRegistrator services, string trackerName, Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<DotNetAssembly>>();
            return new DotNetGitHubScanner(getRepositoriesDelegate, trackedApplication, mediator, parsers, loggerFactory.CreateLogger<DotNetGitHubScanner>());
        });

    private static IScannerRegistrator AddPhpGitHubScanner(this IScannerRegistrator services, string trackerName, Func<IGitHubClient, string, Task<IReadOnlyList<Repository>>> getRepositoriesDelegate)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<PhpModule>>();
            return new PHPGitHubScanner(getRepositoriesDelegate, trackedApplication, mediator, parsers, loggerFactory.CreateLogger<PHPGitHubScanner>());
        });
}
