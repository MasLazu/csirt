using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSummaryAnalytics;

public class GetThreatEventSummaryAnalyticsQueryHandler : IRequestHandler<GetThreatEventSummaryAnalyticsQuery, ThreatEventSummaryAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetThreatEventSummaryAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventSummaryAnalyticsDto> Handle(GetThreatEventSummaryAnalyticsQuery request, CancellationToken ct)
    {
        // Set default time range if not provided and ensure UTC
        DateTime endTime = request.EndTime ?? DateTime.UtcNow;
        DateTime startTime = request.StartTime ?? endTime.AddDays(-30);
        
        // Ensure DateTime values are in UTC for PostgreSQL compatibility
        DateTime endTimeUtc = endTime.Kind == DateTimeKind.Utc ? endTime : endTime.ToUniversalTime();
        DateTime startTimeUtc = startTime.Kind == DateTimeKind.Utc ? startTime : startTime.ToUniversalTime();

        // Get dashboard metrics (includes recent activity)
        ThreatEventDashboardMetrics dashboardMetrics = await _analyticsRepository.GetDashboardMetricsAsync(
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get summary analytics for the specified period
        ThreatEventSummary summary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get categorical analytics
        IEnumerable<CategoryAnalytics> categories = await _analyticsRepository.GetTopCategoriesAsync(
            startTimeUtc,
            endTimeUtc,
            request.TopItemsLimit,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get geographical analytics
        IEnumerable<GeographicalAnalytics> countries = await _analyticsRepository.GetGeographicalAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.TopItemsLimit,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get malware family analytics
        IEnumerable<MalwareFamilyAnalytics> malwareFamilies = await _analyticsRepository.GetMalwareFamilyAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.TopItemsLimit,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get IP reputation analytics for top source IPs
        IEnumerable<IpReputationAnalytics> sourceIPs = await _analyticsRepository.GetIpReputationAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.TopItemsLimit,
            isSourceIp: true,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Get port analytics for top target ports
        IEnumerable<PortAnalytics> targetPorts = await _analyticsRepository.GetPortAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            isSourcePort: false,
            request.TopItemsLimit,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Calculate time periods for trend analysis
        DateTime last24Hours = endTimeUtc.AddDays(-1);
        DateTime last7Days = endTimeUtc.AddDays(-7);
        DateTime last30Days = endTimeUtc.AddDays(-30);

        // Get counts for different periods
        ThreatEventSummary last24HoursSummary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            last24Hours, endTimeUtc, tenantId: null, ct);
        
        ThreatEventSummary last7DaysSummary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            last7Days, endTimeUtc, tenantId: null, ct);
        
        ThreatEventSummary last30DaysSummary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            last30Days, endTimeUtc, tenantId: null, ct);

        // Calculate trend changes
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
                    TopMalwareFamilies = new List<string>(), // Could be enhanced to get from repository
                    TopSourceCountries = new List<string>()  // Could be enhanced to get from repository
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
                    TrendChange = 0 // Could be enhanced with trend calculation
                }).ToList(),
                SourceCountries = countries.Select(c => new TopItemDto
                {
                    Name = c.CountryName,
                    Value = c.CountryCode,
                    Count = c.Count,
                    Percentage = c.Percentage,
                    TrendChange = 0 // Could be enhanced with trend calculation
                }).ToList(),
                TargetedCountries = new List<TopItemDto>(), // Could be enhanced to get destination countries
                SourceIPs = sourceIPs.Select(ip => new TopItemDto
                {
                    Name = ip.IpAddress.ToString(),
                    Value = ip.IpAddress.ToString(),
                    Count = ip.Count,
                    Percentage = 0, // Calculate if needed
                    TrendChange = 0 // Could be enhanced with trend calculation
                }).ToList(),
                TargetPorts = targetPorts.Select(p => new TopItemDto
                {
                    Name = $"Port {p.Port}",
                    Value = p.Port.ToString(),
                    Count = p.Count,
                    Percentage = p.Percentage,
                    TrendChange = 0 // Could be enhanced with trend calculation
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
