using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventCategoryTimelineAnalytics;

public class GetTenantThreatEventCategoryTimelineAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventCategoryTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repo;

    public GetTenantThreatEventCategoryTimelineAnalyticsQueryHandler(IThreatEventAnalyticsRepository repo)
    {
        _repo = repo;
    }

    public async Task<ThreatEventTimelineAnalyticsDto> Handle(GetTenantThreatEventCategoryTimelineAnalyticsQuery request, CancellationToken ct)
    {
        DateTime end = request.EndTime ?? DateTime.UtcNow;
        DateTime start = request.StartTime ?? end.AddDays(-7);
        if (start.Kind != DateTimeKind.Utc) start = start.ToUniversalTime();
        if (end.Kind != DateTimeKind.Utc) end = end.ToUniversalTime();

        var window = end - start;
        DateTime comparisonEnd = start;
        DateTime comparisonStart = comparisonEnd - window;

        var raw = (await _repo.GetTimelineAnalyticsAsync(start, end, request.Interval, request.TenantId, category: request.Category, malwareFamilyId: null, sourceCountryId: null, ct)).ToList();
        var prevRaw = (await _repo.GetTimelineAnalyticsAsync(comparisonStart, comparisonEnd, request.Interval, request.TenantId, category: request.Category, malwareFamilyId: null, sourceCountryId: null, ct)).ToList();

        var grouped = raw
            .GroupBy(r => r.Timestamp)
            .Select(g => new ThreatEventTimelineDto
            {
                Timestamp = g.Key,
                EventCount = g.Sum(x => x.Count),
                Categories = g.ToDictionary(x => x.Category, x => x.Count),
                Severity = new(),
                MalwareFamilies = new()
            }).OrderBy(t => t.Timestamp).ToList();

        var prevGrouped = prevRaw
            .GroupBy(r => r.Timestamp)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

        foreach (var bucket in grouped)
        {
            if (prevGrouped.TryGetValue(bucket.Timestamp - window, out int prevCount))
            {
                double change = prevCount == 0 ? 100 : ((double)bucket.EventCount - prevCount) / prevCount * 100.0;
                bucket.Severity["_trend_pct"] = (int)Math.Round(change);
            }
        }

        int totalCurrent = grouped.Sum(t => t.EventCount);
        int totalPrevious = prevRaw.Sum(r => r.Count);
        double pctChange = totalPrevious == 0 ? 100 : ((double)totalCurrent - totalPrevious) / totalPrevious * 100.0;
        string direction = totalCurrent > totalPrevious ? "increasing" : totalCurrent < totalPrevious ? "decreasing" : "stable";

        return new ThreatEventTimelineAnalyticsDto
        {
            Timeline = grouped,
            TotalEvents = totalCurrent,
            Interval = request.Interval,
            TimeRange = new TimeRangeDto { Start = start, End = end },
            Trends = new TrendAnalysisDto
            {
                PercentageChange = pctChange,
                Direction = direction,
                ComparisonPeriodStart = comparisonStart,
                ComparisonPeriodEnd = comparisonEnd
            }
        };
    }
}
