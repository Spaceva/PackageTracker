using System.Text.Json;

namespace PackageTracker.Cache;

internal abstract class FileContext<TEntity, TDb> : IFileContext
{
    private readonly SemaphoreSlim semaphore = new(1);

    public string FileName { get; }

    public FileContext(string dbFileName)
    {
        FileName = Path.Combine(AppContext.BaseDirectory, dbFileName);
    }

    public async Task<IReadOnlyCollection<TEntity>> ReadAsync()
    {
        await semaphore.WaitAsync();
        try
        {
            if (!File.Exists(FileName))
            {
                return Array.Empty<TEntity>();
            }

            var dbFileContent = await File.ReadAllTextAsync(FileName);
            if (string.IsNullOrWhiteSpace(dbFileContent))
            {
                return Array.Empty<TEntity>();
            }

            var db = JsonSerializer.Deserialize<TDb>(dbFileContent);
            return Entities(db!);
        }
        catch
        {
            return Array.Empty<TEntity>();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task WriteAsync(IReadOnlyCollection<TEntity> entities)
    {
        await semaphore.WaitAsync();
        try
        {
            var dbFileContent = JsonSerializer.Serialize(Db(entities));
            await File.WriteAllTextAsync(FileName, dbFileContent);
        }
        finally
        {
            semaphore.Release();
        }
    }

    protected abstract IReadOnlyCollection<TEntity> Entities(TDb db);

    protected abstract TDb Db(IReadOnlyCollection<TEntity> entities);

    async Task<IReadOnlyCollection<object>> IFileContext.ReadAsync()
    {
        var entities = await ReadAsync();
        return entities.Cast<object>().ToArray();
    }

    Task IFileContext.WriteAsync(IReadOnlyCollection<object> entities)
    {
        return WriteAsync(entities.Cast<TEntity>().ToArray());
    }
}
