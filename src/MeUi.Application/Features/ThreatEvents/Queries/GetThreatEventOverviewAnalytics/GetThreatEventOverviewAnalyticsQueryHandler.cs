using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;

public class GetThreatEventOverviewAnalyticsQueryHandler : IRequestHandler<GetThreatEventOverviewAnalyticsQuery, ThreatEventOverviewAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetThreatEventOverviewAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<ThreatEventOverviewAnalyticsDto> Handle(GetThreatEventOverviewAnalyticsQuery request, CancellationToken ct)
    {
        DateTime endTime = request.EndTime ?? DateTime.UtcNow;
        DateTime startTime = request.StartTime ?? endTime.AddDays(-30);
        DateTime startUtc = startTime.Kind == DateTimeKind.Utc ? startTime : startTime.ToUniversalTime();
        DateTime endUtc = endTime.Kind == DateTimeKind.Utc ? endTime : endTime.ToUniversalTime();
        int top = request.TopItemsLimit;

        // Parallel lightweight aggregates (tenant-scoped if provided)
        Guid? tenantId = null;
        var dashboardTask = _analyticsRepository.GetDashboardMetricsAsync(tenantId, ct);
        var summaryTask = _analyticsRepository.GetSummaryAnalyticsAsync(startUtc, endUtc, tenantId, ct);
        var categoriesTask = _analyticsRepository.GetTopCategoriesAsync(startUtc, endUtc, top, tenantId, ct);
        var malwareTask = _analyticsRepository.GetMalwareFamilyAnalyticsAsync(startUtc, endUtc, top, tenantId, ct);
        var geoTask = _analyticsRepository.GetGeographicalAnalyticsAsync(startUtc, endUtc, top, tenantId, ct);

        await Task.WhenAll(dashboardTask, summaryTask, categoriesTask, malwareTask, geoTask);

        var dashboard = await dashboardTask;
        var summary = await summaryTask;
        var categories = (await categoriesTask).ToList();
        var malwareFamilies = (await malwareTask).ToList();
        var geo = (await geoTask).ToList();

        return new ThreatEventOverviewAnalyticsDto
        {
            Overview = new ThreatEventOverviewStatsDto
            {
                TotalEvents = summary.TotalEvents,
                UniqueSourceIps = summary.UniqueSourceIps,
                UniqueMalwareFamilies = summary.UniqueMalwareFamilies,
                UniqueCountries = summary.UniqueCountries,
                EventsLast24Hours = dashboard.EventsLast24Hours,
                EventsLastHour = dashboard.EventsLastHour,
                ActiveThreatsCurrently = dashboard.ActiveThreatsCurrently,
                CriticalAlertsCount = dashboard.CriticalAlertsCount,
                TopThreatCategory = dashboard.TopThreatCategory,
                TopSourceCountry = dashboard.TopSourceCountry
            },
            TopCategories = categories.Select(c => new TopItemDto
            {
                Name = c.Category,
                Value = c.Category,
                Count = c.Count,
                Percentage = c.Percentage,
                TrendChange = c.PercentageChange
            }).ToList(),
            TopMalwareFamilies = malwareFamilies.Select(m => new TopItemDto
            {
                Name = m.FamilyName,
                Value = m.FamilyName,
                Count = m.Count,
                Percentage = m.Percentage,
                TrendChange = 0 // enhancement placeholder
            }).ToList(),
            TopSourceCountries = geo.Select(g => new TopItemDto
            {
                Name = g.CountryName,
                Value = g.CountryCode,
                Count = g.Count,
                Percentage = g.Percentage,
                TrendChange = 0 // enhancement placeholder
            }).ToList(),
            PercentageChangeFromYesterday = dashboard.PercentageChangeFromYesterday,
            LastUpdated = DateTime.UtcNow,
            TimeRange = new TimeRangeDto { Start = startUtc, End = endUtc }
        };
    }
}
