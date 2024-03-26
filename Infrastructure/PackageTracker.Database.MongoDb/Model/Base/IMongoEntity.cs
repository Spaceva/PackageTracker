using MongoDB.Bson;

namespace PackageTracker.Database.MongoDb.Model.Base;

internal interface IMongoEntity
{
    public ObjectId? Id { get; set; }
}
