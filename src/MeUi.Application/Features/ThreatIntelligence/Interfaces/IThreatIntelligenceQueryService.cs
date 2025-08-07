using MeUi.Application.Features.ThreatIntelligence.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatIntelligence.Interfaces;

public interface IThreatIntelligenceQueryService
{
    /// <summary>
    /// Gets threat intelligence data by filter criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Collection of threat intelligence records</returns>
    Task<IEnumerable<MeUi.Domain.Entities.ThreatIntelligence>> GetByFilterAsync(ThreatIntelligenceFilter filter, CancellationToken ct = default);

    /// <summary>
    /// Gets paginated threat intelligence data by filter criteria
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated result with items and total count</returns>
    Task<(IEnumerable<MeUi.Domain.Entities.ThreatIntelligence> Items, int TotalCount)> GetPaginatedAsync(ThreatIntelligenceFilter filter, CancellationToken ct = default);
}