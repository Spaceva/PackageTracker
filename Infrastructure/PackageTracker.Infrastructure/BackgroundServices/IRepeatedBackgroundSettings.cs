namespace PackageTracker.Infrastructure.BackgroundServices;

public interface IRepeatedBackgroundSettings
{
    TimeSpan TimeBetweenEachExecution { get; }
}
