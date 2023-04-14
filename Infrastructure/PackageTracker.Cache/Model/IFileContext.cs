namespace PackageTracker.Cache;

internal interface IFileContext
{
    string FileName { get; }

    Task<IReadOnlyCollection<object>> ReadAsync();

    Task WriteAsync(IReadOnlyCollection<object> entities);
}
