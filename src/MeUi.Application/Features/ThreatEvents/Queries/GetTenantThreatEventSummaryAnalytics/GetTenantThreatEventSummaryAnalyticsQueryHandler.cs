using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventSummaryAnalytics;

public class GetTenantThreatEventSummaryAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventSummaryAnalyticsQuery, ThreatEventSummaryAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetTenantThreatEventSummaryAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventSummaryAnalyticsDto> Handle(GetTenantThreatEventSummaryAnalyticsQuery request, CancellationToken ct)
    {
        DateTime endTime = request.EndTime ?? DateTime.UtcNow;
        DateTime startTime = request.StartTime ?? endTime.AddDays(-30);
        DateTime endTimeUtc = endTime.Kind == DateTimeKind.Utc ? endTime : endTime.ToUniversalTime();
        DateTime startTimeUtc = startTime.Kind == DateTimeKind.Utc ? startTime : startTime.ToUniversalTime();
        int top = request.TopItemsLimit;

        Guid? tenantId = request.TenantId;
        var dashboardTask = _analyticsRepository.GetDashboardMetricsAsync(tenantId, ct);
        var summaryTask = _analyticsRepository.GetSummaryAnalyticsAsync(startTimeUtc, endTimeUtc, tenantId, ct);
        var categoriesTask = _analyticsRepository.GetTopCategoriesAsync(startTimeUtc, endTimeUtc, top, tenantId, ct);
        var countriesTask = _analyticsRepository.GetGeographicalAnalyticsAsync(startTimeUtc, endTimeUtc, top, tenantId, ct);
        var malwareFamiliesTask = _analyticsRepository.GetMalwareFamilyAnalyticsAsync(startTimeUtc, endTimeUtc, top, tenantId, ct);
        var sourceIpsTask = _analyticsRepository.GetIpReputationAnalyticsAsync(startTimeUtc, endTimeUtc, top, isSourceIp: true, tenantId, ct);
        var targetPortsTask = _analyticsRepository.GetPortAnalyticsAsync(startTimeUtc, endTimeUtc, isSourcePort: false, top, tenantId, ct);

        DateTime last24Hours = endTimeUtc.AddDays(-1);
        DateTime last7Days = endTimeUtc.AddDays(-7);
        DateTime last30Days = endTimeUtc.AddDays(-30);
        var last24Task = _analyticsRepository.GetSummaryAnalyticsAsync(last24Hours, endTimeUtc, tenantId, ct);
        var last7Task = _analyticsRepository.GetSummaryAnalyticsAsync(last7Days, endTimeUtc, tenantId, ct);
        var last30Task = _analyticsRepository.GetSummaryAnalyticsAsync(last30Days, endTimeUtc, tenantId, ct);

        await Task.WhenAll(dashboardTask, summaryTask, categoriesTask, countriesTask, malwareFamiliesTask,
            sourceIpsTask, targetPortsTask, last24Task, last7Task, last30Task);

        var dashboardMetrics = await dashboardTask;
        var summary = await summaryTask;
        var categories = await categoriesTask;
        var countries = await countriesTask;
        var malwareFamilies = await malwareFamiliesTask;
        var sourceIPs = await sourceIpsTask;
        var targetPorts = await targetPortsTask;
        var last24HoursSummary = await last24Task;
        var last7DaysSummary = await last7Task;
        var last30DaysSummary = await last30Task;

        double dailyChange = CalculatePercentageChange(dashboardMetrics.EventsLast24Hours, dashboardMetrics.EventsLastHour * 24);
        string trendDirection = dashboardMetrics.PercentageChangeFromYesterday > 5 ? "up" :
                               dashboardMetrics.PercentageChangeFromYesterday < -5 ? "down" : "stable";

        return new ThreatEventSummaryAnalyticsDto
        {
            Overview = new ThreatOverviewDto
            {
                TotalEvents = summary.TotalEvents,
                Last24Hours = last24HoursSummary.TotalEvents,
                Last7Days = last7DaysSummary.TotalEvents,
                Last30Days = last30DaysSummary.TotalEvents,
                ActiveSourceIPs = summary.UniqueSourceIps,
                AffectedCountries = summary.UniqueCountries,
                UniqueMalwareFamilies = summary.UniqueMalwareFamilies
            },
            TrendIndicators = new TrendIndicatorsDto
            {
                DailyChange = dailyChange,
                WeeklyChange = CalculatePercentageChange(last7DaysSummary.TotalEvents, last24HoursSummary.TotalEvents * 7),
                MonthlyChange = dashboardMetrics.PercentageChangeFromYesterday,
                TrendDirection = trendDirection
            },
            ThreatCategories = categories.ToDictionary(
                c => c.Category,
                c => new CategoryStatsDto
                {
                    Count = c.Count,
                    Percentage = c.Percentage,
                    TrendChange = c.PercentageChange,
                    TopMalwareFamilies = new List<string>(),
                    TopSourceCountries = new List<string>()
                }),
            CriticalAlerts = request.IncludeCriticalAlerts
                ? dashboardMetrics.RecentHighRiskEvents.Select(e => new CriticalAlertDto
                {
                    Type = "high_risk_event",
                    Message = $"High-risk threat detected from {e.SourceAddress} ({e.CountryName}) - {e.MalwareFamilyName}",
                    Timestamp = e.Timestamp,
                    Severity = e.RiskScore >= 8.0 ? "critical" : e.RiskScore >= 6.0 ? "high" : "medium",
                    Metadata = new Dictionary<string, object>
                    {
                        ["sourceAddress"] = e.SourceAddress.ToString(),
                        ["category"] = e.Category,
                        ["riskScore"] = e.RiskScore,
                        ["countryName"] = e.CountryName,
                        ["malwareFamilyName"] = e.MalwareFamilyName
                    }
                }).ToList()
                : new List<CriticalAlertDto>(),
            TopThreats = new TopThreatsDto
            {
                MalwareFamilies = malwareFamilies.Select(m => new TopItemDto
                {
                    Name = m.FamilyName,
                    Value = m.FamilyName,
                    Count = m.Count,
                    Percentage = m.Percentage,
                    TrendChange = 0
                }).ToList(),
                SourceCountries = countries.Select(c => new TopItemDto
                {
                    Name = c.CountryName,
                    Value = c.CountryCode,
                    Count = c.Count,
                    Percentage = c.Percentage,
                    TrendChange = 0
                }).ToList(),
                TargetedCountries = new List<TopItemDto>(),
                SourceIPs = sourceIPs.Select(ip => new TopItemDto
                {
                    Name = ip.IpAddress.ToString(),
                    Value = ip.IpAddress.ToString(),
                    Count = ip.Count,
                    Percentage = 0,
                    TrendChange = 0
                }).ToList(),
                TargetPorts = targetPorts.Select(p => new TopItemDto
                {
                    Name = $"Port {p.Port}",
                    Value = p.Port.ToString(),
                    Count = p.Count,
                    Percentage = p.Percentage,
                    TrendChange = 0
                }).ToList()
            },
            LastUpdated = DateTime.UtcNow
        };
    }

    private static double CalculatePercentageChange(int current, int previous)
    {
        if (previous == 0)
        {
            return current > 0 ? 100.0 : 0.0;
        }
        return (double)(current - previous) / previous * 100.0;
    }
}
