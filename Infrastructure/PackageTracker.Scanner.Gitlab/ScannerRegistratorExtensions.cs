namespace PackageTracker.Scanner.Gitlab;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddGitlabScanner(this IScannerRegistrator services, string trackerName)
    => services.Register<GitlabScanner>(trackerName, (sp, settings, trackedApplication, parsers, logger, mediator) => new GitlabScanner(trackedApplication, parsers, logger, mediator));
}
