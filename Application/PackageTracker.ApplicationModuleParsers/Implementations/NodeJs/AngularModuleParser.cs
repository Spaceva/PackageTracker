using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ApplicationModuleParsers;

internal class AngularModuleParser(IPackagesRepository packagesRepository, ILogger<AngularModuleParser> logger) : NodeModuleParser(packagesRepository, logger)
{
    protected override string FrameworkPackageName => Constants.Application.NodeJs.AngularVersionPropertyName;

    protected override ApplicationModule ApplicationModule(string moduleName, string frameworkVersion, ApplicationPackage[] packages)
        => new AngularModule { Name = moduleName, FrameworkVersion = frameworkVersion, Packages = packages };
}
