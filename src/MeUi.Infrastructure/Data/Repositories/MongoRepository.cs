using System.Linq.Expressions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;
using MeUi.Infrastructure.Data.Configurations;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MeUi.Infrastructure.Data.Repositories;

public class MongoRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database)
    {
        var collectionName = MongoDbConfiguration.GetCollectionName<T>();
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, id),
            Builders<T>.Filter.Eq(x => x.DeletedAt, null)
        );

        return await _collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        return await _collection.Find(filter).ToListAsync(ct);
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<T>.Filter.Where(predicate);
        var combinedFilter = Builders<T>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).ToListAsync(ct);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<T>.Filter.Where(predicate);
        var combinedFilter = Builders<T>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).FirstOrDefaultAsync(ct);
    }

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = null;
        entity.DeletedAt = null;

        await _collection.InsertOneAsync(entity, cancellationToken: ct);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
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

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, entity.Id),
            Builders<T>.Filter.Eq(x => x.DeletedAt, null)
        );

        await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        var update = Builders<T>.Update
            .Set(x => x.DeletedAt, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
    {
        var ids = entities.Select(x => x.Id).ToList();
        var filter = Builders<T>.Filter.In(x => x.Id, ids);
        var update = Builders<T>.Update
            .Set(x => x.DeletedAt, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(filter, update, cancellationToken: ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, id),
            Builders<T>.Filter.Eq(x => x.DeletedAt, null)
        );

        return await _collection.Find(filter).AnyAsync(ct);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<T>.Filter.Where(predicate);
        var combinedFilter = Builders<T>.Filter.And(softDeleteFilter, predicateFilter);

        return await _collection.Find(combinedFilter).AnyAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        var predicateFilter = Builders<T>.Filter.Where(predicate);
        var combinedFilter = Builders<T>.Filter.And(softDeleteFilter, predicateFilter);

        return (int)await _collection.CountDocumentsAsync(combinedFilter, cancellationToken: ct);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default)
    {
        var softDeleteFilter = Builders<T>.Filter.Eq(x => x.DeletedAt, null);
        var filter = softDeleteFilter;

        if (predicate != null)
        {
            var predicateFilter = Builders<T>.Filter.Where(predicate);
            filter = Builders<T>.Filter.And(softDeleteFilter, predicateFilter);
        }

        var totalCount = (int)await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var findFluent = _collection.Find(filter).Skip(skip).Limit(take);

        if (orderBy != null)
        {
            var sortDefinition = orderByDescending
                ? Builders<T>.Sort.Descending(orderBy)
                : Builders<T>.Sort.Ascending(orderBy);
            findFluent = findFluent.Sort(sortDefinition);
        }

        var items = await findFluent.ToListAsync(ct);

        return (items, totalCount);
    }

    public IQueryable<T> Query()
    {
        return _collection.AsQueryable().Where(x => x.DeletedAt == null);
    }
}