using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public static class MongoDbConfiguration
{
    private static bool _isConfigured = false;
    private static bool _indexesCreated = false;

    public static void Configure()
    {
        if (_isConfigured)
            return;

        // Configure conventions
        ConfigureConventions();

        // Configure entity mappings
        ConfigureBaseEntityMapping();
        ConfigureThreatIntelligenceMapping();
        ConfigureOptionalInformationMapping();

        _isConfigured = true;
    }

    public static async Task ConfigureIndexesAsync(IMongoDatabase database)
    {
        if (_indexesCreated)
            return;

        await CreateThreatIntelligenceIndexesAsync(database);
        _indexesCreated = true;
    }

    public static async Task EnsureIndexesAsync(IMongoDatabase database)
    {
        // Force index creation even if already marked as created
        // Useful for manual index management or troubleshooting
        await CreateThreatIntelligenceIndexesAsync(database);
        _indexesCreated = true;
    }

    public static string GetCollectionName<T>()
    {
        return typeof(T).Name switch
        {
            nameof(ThreatIntelligence) => "ThreatIntelligence",
            _ => typeof(T).Name
        };
    }

    private static void ConfigureConventions()
    {
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new IgnoreIfNullConvention(true)
        };

        ConventionRegistry.Register("ThreatIntelligenceConventions", conventionPack, t => true);
    }

    private static void ConfigureBaseEntityMapping()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
        {
            BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
            {
                cm.AutoMap();

                // Configure Id property - convert between MongoDB ObjectId and Guid
                cm.MapIdProperty(x => x.Id)
                  .SetIdGenerator(new GuidGenerator())
                  .SetSerializer(new GuidSerializer(BsonType.String));

                // Map audit properties
                cm.MapProperty(x => x.CreatedAt)
                  .SetElementName("created_at")
                  .SetSerializer(new DateTimeSerializer(BsonType.DateTime))
                  .SetDefaultValue(DateTime.UtcNow);
                cm.MapProperty(x => x.UpdatedAt)
                  .SetElementName("updated_at")
                  .SetSerializer(new NullableSerializer<DateTime>(new DateTimeSerializer(BsonType.DateTime)))
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.DeletedAt)
                  .SetElementName("deleted_at")
                  .SetSerializer(new NullableSerializer<DateTime>(new DateTimeSerializer(BsonType.DateTime)))
                  .SetIgnoreIfNull(true);

                // Ignore computed property
                cm.UnmapProperty(x => x.IsDeleted);
            });
        }
    }

    private static void ConfigureThreatIntelligenceMapping()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ThreatIntelligence)))
        {
            BsonClassMap.RegisterClassMap<ThreatIntelligence>(cm =>
            {
                cm.AutoMap();

                // Map MongoDB field names to C# properties specific to ThreatIntelligence
                cm.MapProperty(x => x.Asn).SetElementName("asn");
                cm.MapProperty(x => x.Timestamp)
                  .SetElementName("timestamp")
                  .SetSerializer(new DateTimeSerializer(BsonType.DateTime));
                cm.MapProperty(x => x.AsnInfo).SetElementName("asninfo");
                cm.MapProperty(x => x.OptionalInformation).SetElementName("optional_information");
                cm.MapProperty(x => x.Category).SetElementName("category");
                cm.MapProperty(x => x.SourceAddress).SetElementName("source_address");
                cm.MapProperty(x => x.SourceCountry).SetElementName("source_country");
            });
        }
    }

    private static void ConfigureOptionalInformationMapping()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(OptionalInformation)))
        {
            BsonClassMap.RegisterClassMap<OptionalInformation>(cm =>
            {
                cm.AutoMap();

                // Map MongoDB field names to C# properties
                cm.MapProperty(x => x.DestinationAddress)
                  .SetElementName("destination_address")
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.DestinationCountry)
                  .SetElementName("destination_country")
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.DestinationPort)
                  .SetElementName("destination_port")
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.SourcePort)
                  .SetElementName("source_port")
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.Protocol)
                  .SetElementName("protocol")
                  .SetIgnoreIfNull(true);
                cm.MapProperty(x => x.Family)
                  .SetElementName("family")
                  .SetIgnoreIfNull(true);
            });
        }
    }

    private static async Task CreateThreatIntelligenceIndexesAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<ThreatIntelligence>(GetCollectionName<ThreatIntelligence>());

        // Create single field indexes for optimal query performance
        var indexModels = new List<CreateIndexModel<ThreatIntelligence>>
        {
            // Single field indexes
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("asn"),
                new CreateIndexOptions { Name = "idx_asn" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("source_address"),
                new CreateIndexOptions { Name = "idx_source_address" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Descending("timestamp"),
                new CreateIndexOptions { Name = "idx_timestamp_desc" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("category"),
                new CreateIndexOptions { Name = "idx_category" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("source_country"),
                new CreateIndexOptions { Name = "idx_source_country" }),

            // Compound indexes for common filter combinations
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys
                    .Ascending("source_country")
                    .Descending("timestamp"),
                new CreateIndexOptions { Name = "idx_source_country_timestamp" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys
                    .Ascending("category")
                    .Descending("timestamp"),
                new CreateIndexOptions { Name = "idx_category_timestamp" }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys
                    .Ascending("asn")
                    .Descending("timestamp"),
                new CreateIndexOptions { Name = "idx_asn_timestamp" }),

            // Nested field indexes for optional information
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("optional_information.protocol"),
                new CreateIndexOptions { Name = "idx_protocol", Sparse = true }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("optional_information.destination_address"),
                new CreateIndexOptions { Name = "idx_destination_address", Sparse = true }),

            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("optional_information.family"),
                new CreateIndexOptions { Name = "idx_family", Sparse = true }),

            // Soft delete index for BaseEntity
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys.Ascending("deleted_at"),
                new CreateIndexOptions { Name = "idx_deleted_at", Sparse = true }),

            // Compound index for time range queries with category
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys
                    .Ascending("category")
                    .Ascending("timestamp")
                    .Ascending("deleted_at"),
                new CreateIndexOptions { Name = "idx_category_timestamp_deleted" }),

            // Compound index for complex filtering scenarios
            new CreateIndexModel<ThreatIntelligence>(
                Builders<ThreatIntelligence>.IndexKeys
                    .Ascending("source_country")
                    .Ascending("category")
                    .Descending("timestamp"),
                new CreateIndexOptions { Name = "idx_country_category_timestamp" })
        };

        try
        {
            await collection.Indexes.CreateManyAsync(indexModels);
        }
        catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.CodeName == "IndexKeySpecsConflict")
        {
            // Indexes already exist, which is fine
            // This can happen during development or multiple deployments
        }
    }
}