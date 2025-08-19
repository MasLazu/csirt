using MeUi.Application.Models.ThreatIncident;

namespace MeUi.Application.Interfaces;

public interface IThreatIncidentRepository
{
    Task<List<IncidentSummaryDto>> GetActiveIncidentsAsync(DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<SeverityDistributionDto>> GetSeverityDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<ResponseTimeMetricDto>> GetResponseTimeMetricsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
