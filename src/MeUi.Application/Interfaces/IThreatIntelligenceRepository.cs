using System.Linq.Expressions;
using MeUi.Domain.Entities;

namespace MeUi.Application.Interfaces;

public interface IThreatIntelligenceRepository
{
    Task<ThreatIntelligence?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ThreatIntelligence>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<ThreatIntelligence>> FindAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default);
    Task<ThreatIntelligence?> FirstOrDefaultAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default);
    Task<ThreatIntelligence> AddAsync(ThreatIntelligence entity, CancellationToken ct = default);
    Task<IEnumerable<ThreatIntelligence>> AddRangeAsync(IEnumerable<ThreatIntelligence> entities, CancellationToken ct = default);
    Task UpdateAsync(ThreatIntelligence entity, CancellationToken ct = default);
    Task DeleteAsync(ThreatIntelligence entity, CancellationToken ct = default);
    Task DeleteRangeAsync(IEnumerable<ThreatIntelligence> entities, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<ThreatIntelligence, bool>> predicate, CancellationToken ct = default);
    Task<(IEnumerable<ThreatIntelligence> Items, int TotalCount)> GetPaginatedAsync(
        Expression<Func<ThreatIntelligence, bool>>? predicate = null,
        Expression<Func<ThreatIntelligence, object>>? orderBy = null,
        bool orderByDescending = false,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default);
    IQueryable<ThreatIntelligence> Query();
}