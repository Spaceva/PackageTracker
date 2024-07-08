using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;

namespace PackageTracker.Monitor.EndOfLife;
internal class AngularEndOfLifeMonitor(ILogger<AngularEndOfLifeMonitor> logger, IOptions<MonitorSettings> options, IPackagesRepository packagesRepository) : NpmFrameworkEndOfLifeMonitor("angular", logger, options, packagesRepository)
{
    protected override string FrameworkPackageName => "@angular/cli";

    protected override string FrameworkName => AngularModule.FrameworkName;
}
