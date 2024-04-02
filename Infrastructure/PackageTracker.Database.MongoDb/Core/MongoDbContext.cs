using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace PackageTracker.Database.MongoDb.Core;
internal class MongoDbContext
{
    private readonly IMongoDatabase? database;

    public MongoDbContext(string connectionString)
    {
        ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, t => true);
        ConventionRegistry.Register("Ignored", new ConventionPack { new IgnoreIfNullConvention(true), new IgnoreExtraElementsConvention(true) }, t => true);
        ConventionRegistry.Register("EnumRepresentation", new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, t => true);
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var url = new MongoUrl(connectionString);
        var client = new MongoClient(url);
        database = client.GetDatabase(url.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        return database!.GetCollection<T>(typeof(T).AsCollectionName());
    }
}
