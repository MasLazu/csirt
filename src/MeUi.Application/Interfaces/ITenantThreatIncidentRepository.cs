using MeUi.Application.Models.ThreatIncident;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatIncidentRepository
{
    Task<List<IncidentStatusDto>> GetActiveIncidentStatusAsync(Guid tenantId, DateTime start, DateTime end, int limit = 30, CancellationToken cancellationToken = default);
    Task<List<IncidentSeverityDistributionDto>> GetIncidentSeverityDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<ResponseTimeMetricDto>> GetResponseTimeMetricsAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
}