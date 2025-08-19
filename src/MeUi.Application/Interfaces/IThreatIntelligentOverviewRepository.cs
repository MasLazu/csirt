using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeUi.Application.Models.ThreatIntelligentOverview;

namespace MeUi.Application.Interfaces
{
    public interface IThreatIntelligentOverviewRepository
    {
        Task<List<ExecutiveSummaryMetricDto>> GetExecutiveSummaryAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<List<TimelineDataPointDto>> GetThreatActivityTimelineAsync(DateTime start, DateTime end, TimeSpan interval, CancellationToken cancellationToken = default);
        Task<List<TopCategoryDto>> GetTopThreatCategoriesAsync(DateTime start, DateTime end, int limit = 5, CancellationToken cancellationToken = default);
        Task<List<TopCountryDto>> GetTopSourceCountriesAsync(DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default);
        Task<List<ProtocolDistributionDto>> GetProtocolDistributionAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
        Task<List<HighRiskSourceIpDto>> GetHighRiskSourceIpsAsync(DateTime start, DateTime end, int limit = 15, CancellationToken cancellationToken = default);
        Task<List<TargetedPortDto>> GetTopTargetedPortsAsync(DateTime start, DateTime end, int limit = 10, CancellationToken cancellationToken = default);
        Task<List<ThreatCategoryAnalysisDto>> GetThreatCategoryAnalysisAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    }
}
