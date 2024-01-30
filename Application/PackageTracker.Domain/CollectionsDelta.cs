namespace PackageTracker.Domain;

using System.Linq;

public class CollectionsDelta<TEntity>(IEnumerable<TEntity> updatedEntities, IEnumerable<TEntity> updatingEntities, IEqualityComparer<TEntity> comparer)
{
    public IReadOnlyCollection<TEntity> AddedEntities => [.. updatedEntities.Except(updatingEntities, comparer)];
    public IReadOnlyCollection<TEntity> RemovedEntities => [.. updatingEntities.Except(updatedEntities, comparer)];
    public IReadOnlyCollection<TEntity> UpdatingEntitiesCommon => [.. updatingEntities.Intersect(updatedEntities, comparer)];
    public IReadOnlyCollection<TEntity> UpdatedEntitiesCommon => [.. updatedEntities.Intersect(updatingEntities, comparer)];
}
