using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventDashboardMetrics;

public class GetTenantThreatEventDashboardMetricsQueryHandler : IRequestHandler<GetTenantThreatEventDashboardMetricsQuery, ThreatEventDashboardMetricsDto>
{
    private readonly IThreatEventAnalyticsRepository _repo;

    public GetTenantThreatEventDashboardMetricsQueryHandler(IThreatEventAnalyticsRepository repo)
    {
        _repo = repo;
    }

    public async Task<ThreatEventDashboardMetricsDto> Handle(GetTenantThreatEventDashboardMetricsQuery request, CancellationToken ct)
    {
        var metrics = await _repo.GetDashboardMetricsAsync(request.TenantId, ct);
        return new ThreatEventDashboardMetricsDto
        {
            EventsLast24Hours = metrics.EventsLast24Hours,
            EventsLastHour = metrics.EventsLastHour,
            PercentageChangeFromYesterday = metrics.PercentageChangeFromYesterday,
            ActiveThreatsCurrently = metrics.ActiveThreatsCurrently,
            CriticalAlertsCount = metrics.CriticalAlertsCount,
            TopThreatCategory = metrics.TopThreatCategory,
            TopSourceCountry = metrics.TopSourceCountry,
            RecentHighRiskEvents = metrics.RecentHighRiskEvents.Select(e => new RecentHighRiskEventDto
            {
                EventId = e.EventId,
                Timestamp = e.Timestamp,
                SourceAddress = e.SourceAddress.ToString(),
                Category = e.Category,
                MalwareFamilyName = e.MalwareFamilyName,
                RiskScore = e.RiskScore,
                CountryName = e.CountryName
            }).ToList()
        };
    }
}
