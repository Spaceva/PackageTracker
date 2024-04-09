namespace PackageTracker.Database.MemoryCache.Cloners;

internal abstract class BaseCloner<T>
{
    public abstract T Clone(T duplicatedObject);
}
