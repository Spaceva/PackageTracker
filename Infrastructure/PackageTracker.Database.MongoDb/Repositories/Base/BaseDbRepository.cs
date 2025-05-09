﻿using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model.Base;

namespace PackageTracker.Database.MongoDb.Repositories.Base;
internal abstract class BaseDbRepository<TMongoEntity>(MongoDbContext dbContext, ILogger logger)
    where TMongoEntity : IMongoEntity
{
    protected static readonly FilterDefinitionBuilder<TMongoEntity> Filter = Builders<TMongoEntity>.Filter;

    protected IMongoCollection<TMongoEntity> Collection => dbContext.GetCollection<TMongoEntity>();

    protected ILogger Logger => logger;

    protected MongoDbContext DbContext => dbContext;

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

    public async Task<TMongoEntity> InsertAsync(TMongoEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken: cancellationToken);

        return entity;
    }

    public async Task<IEnumerable<TResult>> ExecutePipelineAsync<TResult>(PipelineDefinition<TMongoEntity, TResult> pipeline, CancellationToken cancellationToken = default)
    {
        var cursor = await Collection.AggregateAsync(pipeline, cancellationToken: cancellationToken);

        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(FilterDefinition<TMongoEntity> predicate, CancellationToken cancellationToken = default)
    {
        var count = await Collection.CountDocumentsAsync(predicate, new CountOptions { Limit = 1 }, cancellationToken);

        return count > 0;
    }
}
