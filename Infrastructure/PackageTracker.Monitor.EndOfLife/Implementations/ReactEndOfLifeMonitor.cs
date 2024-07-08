using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;

namespace PackageTracker.Monitor.EndOfLife;
internal class ReactEndOfLifeMonitor(ILogger<ReactEndOfLifeMonitor> logger, IOptions<MonitorSettings> options, IPackagesRepository packagesRepository) : NpmFrameworkEndOfLifeMonitor("react", logger, options, packagesRepository)
{
    protected override string FrameworkPackageName => "react";

    protected override string FrameworkName => ReactModule.FrameworkName;
}
