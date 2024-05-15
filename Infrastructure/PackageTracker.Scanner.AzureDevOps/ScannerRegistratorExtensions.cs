namespace PackageTracker.Scanner.AzureDevOps;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddAzureDevOpsScanner(this IScannerRegistrator services, string trackerName)
    => services.Register<AzureDevOpsScanner>(trackerName, (sp, settings, trackedApplication, parsers, logger, mediator) => new AzureDevOpsScanner(settings, trackedApplication, parsers, logger, mediator));
}
