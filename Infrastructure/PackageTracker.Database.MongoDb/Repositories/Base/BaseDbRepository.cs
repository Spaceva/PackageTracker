using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model.Base;

namespace PackageTracker.Database.MongoDb.Repositories.Base;
internal abstract class BaseDbRepository<TMongoEntity>(MongoDbContext dbContext, ILogger logger)
    where TMongoEntity : IMongoEntity
{
    protected static readonly FilterDefinitionBuilder<TMongoEntity> Filter = Builders<TMongoEntity>.Filter;

    protected readonly IMongoCollection<TMongoEntity> Collection = dbContext.GetCollection<TMongoEntity>();

    protected readonly ILogger Logger = logger;

    public async Task<long> CountAsync(FilterDefinition<TMongoEntity>? predicate, CancellationToken cancellationToken)
    {
        return await Collection.CountDocumentsAsync(predicate ?? (FilterDefinition<TMongoEntity>.Empty), cancellationToken: cancellationToken);
    }

    public async Task DeleteByQueryAsync(FilterDefinition<TMongoEntity> predicate, CancellationToken cancellationToken = default)
    {
        await Collection.DeleteManyAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TMongoEntity>> FindAsync(FilterDefinition<TMongoEntity> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var cursor = await Collection.FindAsync(predicate, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<TMongoEntity?> GetAsync(FilterDefinition<TMongoEntity> predicate, CancellationToken cancellationToken = default)
    {
        var cursor = await Collection.FindAsync(predicate, cancellationToken: cancellationToken);

        return await cursor.SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<TMongoEntity> UpdateAsync(FilterDefinition<TMongoEntity> predicate, TMongoEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Collection.ReplaceOneAsync(predicate, entity, new ReplaceOptions { IsUpsert = true }, cancellationToken: cancellationToken);

        return entity;
    }
}
