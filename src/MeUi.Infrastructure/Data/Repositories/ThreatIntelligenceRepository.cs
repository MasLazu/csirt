using System.Linq.Expressions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data.Configurations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MeUi.Infrastructure.Data.Repositories;

public class ThreatIntelligenceRepository : IThreatIntelligenceRepository
{
    private readonly IMongoCollection<ThreatIntelligence> _collection;

    public ThreatIntelligenceRepository(IMongoDatabase database)
    {
        var collectionName = MongoDbConfiguration.GetCollectionName<ThreatIntelligence>();
        _collection = database.GetCollection<ThreatIntelligence>(collectionName);
    }

    public async Task<ThreatIntelligence?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<ThreatIntelligence>.Filter.And(
            Builders<ThreatIntelligence>.Filter.Eq(x => x.Id, id),
            Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null)
        );

        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<ThreatIntelligence>> GetAllAsync(CancellationToken ct = default)
    {
        var filter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        return await _collection.Find(filter).ToListAsync(ct);
    }

    public async Task<IEnumerable<ThreatIntelligence>> FindAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<ThreatIntelligence>.Filter.Where(predicate);
        var combinedFilter = Builders<ThreatIntelligence>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).ToListAsync(ct);
    }

    public async Task<ThreatIntelligence?> FirstOrDefaultAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<ThreatIntelligence>.Filter.Where(predicate);
        var combinedFilter = Builders<ThreatIntelligence>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).FirstOrDefaultAsync(ct);
    }

    public async Task<ThreatIntelligence> AddAsync(ThreatIntelligence entity, CancellationToken ct = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = null;
        entity.DeletedAt = null;

        await _collection.InsertOneAsync(entity, cancellationToken: ct);
        return entity;
    }

    public async Task<IEnumerable<ThreatIntelligence>> AddRangeAsync(IEnumerable<ThreatIntelligence> entities, CancellationToken ct = default)
    {
        var entitiesList = entities.ToList();
        var now = DateTime.UtcNow;

        foreach (var entity in entitiesList)
        {
            entity.CreatedAt = now;
            entity.UpdatedAt = null;
            entity.DeletedAt = null;
        }

        await _collection.InsertManyAsync(entitiesList, cancellationToken: ct);
        return entitiesList;
    }

    public async Task UpdateAsync(ThreatIntelligence entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        var filter = Builders<ThreatIntelligence>.Filter.And(
            Builders<ThreatIntelligence>.Filter.Eq(x => x.Id, entity.Id),
            Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null)
        );

        await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
    }

    public async Task DeleteAsync(ThreatIntelligence entity, CancellationToken ct = default)
    {
        var filter = Builders<ThreatIntelligence>.Filter.Eq(x => x.Id, entity.Id);
        var update = Builders<ThreatIntelligence>.Update
            .Set(x => x.DeletedAt, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task DeleteRangeAsync(IEnumerable<ThreatIntelligence> entities, CancellationToken ct = default)
    {
        var ids = entities.Select(x => x.Id).ToList();
        var filter = Builders<ThreatIntelligence>.Filter.In(x => x.Id, ids);
        var update = Builders<ThreatIntelligence>.Update
            .Set(x => x.DeletedAt, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(filter, update, cancellationToken: ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<ThreatIntelligence>.Filter.And(
            Builders<ThreatIntelligence>.Filter.Eq(x => x.Id, id),
            Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null)
        );

        return await _collection.Find(filter).AnyAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<ThreatIntelligence>.Filter.Where(predicate);
        var combinedFilter = Builders<ThreatIntelligence>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).AnyAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        var filter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<int> CountAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<ThreatIntelligence>.Filter.Where(predicate);
        var combinedFilter = Builders<ThreatIntelligence>.Filter.And(softDeleteFilter, predicateFilter);

        return (int)await _collection.CountDocumentsAsync(combinedFilter, cancellationToken: ct);
    }

    public async Task<(IEnumerable<ThreatIntelligence> Items, int TotalCount)> GetPaginatedAsync(
        Expression<Func<ThreatIntelligence, bool>>? predicate = null,
        Expression<Func<ThreatIntelligence, object>>? orderBy = null,
        bool orderByDescending = false,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<ThreatIntelligence>.Filter.Eq(x => x.DeletedAt, null);
        var filter = softDeleteFilter;

        if (predicate != null)
        {
            var predicateFilter = Builders<ThreatIntelligence>.Filter.Where(predicate);
            filter = Builders<ThreatIntelligence>.Filter.And(softDeleteFilter, predicateFilter);
        }

        var totalCount = (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var findFluent = _collection.Find(filter).Skip(skip).Limit(take);

        if (orderBy != null)
        {
            var sortDefinition = orderByDescending
                ? Builders<ThreatIntelligence>.Sort.Descending(orderBy)
                : Builders<ThreatIntelligence>.Sort.Ascending(orderBy);
            findFluent = findFluent.Sort(sortDefinition);
        }

        var items = await findFluent.ToListAsync(ct);

        return (items, totalCount);
    }

    public IQueryable<ThreatIntelligence> Query()
    {
        return _collection.AsQueryable().Where(x => x.DeletedAt == null);
    }
}