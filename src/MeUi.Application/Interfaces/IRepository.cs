using System.Linq.Expressions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default);
    IQueryable<T> Query();

    // Select overloads for existing methods
    Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector, CancellationToken ct = default);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken ct = default);
    Task<IEnumerable<TResult>> FindAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        CancellationToken ct = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        CancellationToken ct = default);
    Task<(IEnumerable<TResult> Items, int TotalCount)> GetPaginatedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool orderByDescending = false,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default);
}