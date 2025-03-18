namespace PackageTracker.Infrastructure.Modules;

public interface IModuleBackgroundSettings
{
    TimeSpan TimeBetweenEachExecution { get; }
}
