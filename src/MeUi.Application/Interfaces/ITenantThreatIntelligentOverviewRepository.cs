using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatIntelligentOverview;

namespace MeUi.Application.Interfaces;

public interface ITenantThreatIntelligentOverviewRepository
{
    Task<List<ExecutiveSummaryMetricDto>> GetExecutiveSummaryAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<TimelineDataPointDto>> GetThreatActivityTimelineAsync(Guid tenantId, DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
    Task<List<TopCategoryDto>> GetTopThreatCategoriesAsync(Guid tenantId, DateTime start, DateTime end, int limit = 5, CancellationToken cancellationToken = default);
    Task<List<TopCountryDto>> GetTopSourceCountriesAsync(Guid tenantId, DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default);
    Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<List<HighRiskSourceIpDto>> GetHighRiskSourceIpsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
    Task<List<TargetedPortDto>> GetTopTargetedPortsAsync(Guid tenantId, DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default);
    Task<List<ThreatCategoryAnalysisDto>> GetThreatCategoryAnalysisAsync(Guid tenantId, DateTime start, DateTime end, CancellationToken cancellationToken = default);
}