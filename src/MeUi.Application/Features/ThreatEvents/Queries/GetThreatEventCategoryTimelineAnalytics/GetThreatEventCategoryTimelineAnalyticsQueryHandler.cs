using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventCategoryTimelineAnalytics;

public class GetThreatEventCategoryTimelineAnalyticsQueryHandler : IRequestHandler<GetThreatEventCategoryTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repo;

    public GetThreatEventCategoryTimelineAnalyticsQueryHandler(IThreatEventAnalyticsRepository repo)
    {
        _repo = repo;
    }

    public async Task<ThreatEventTimelineAnalyticsDto> Handle(GetThreatEventCategoryTimelineAnalyticsQuery request, CancellationToken ct)
    {
        DateTime end = request.EndTime ?? DateTime.UtcNow;
        DateTime start = request.StartTime ?? end.AddDays(-7);
        if (start.Kind != DateTimeKind.Utc) start = start.ToUniversalTime();
        if (end.Kind != DateTimeKind.Utc) end = end.ToUniversalTime();

        // Derive comparison window (same duration immediately preceding current window)
        var window = end - start;
        DateTime comparisonEnd = start;
        DateTime comparisonStart = comparisonEnd - window;

        var raw = (await _repo.GetTimelineAnalyticsAsync(start, end, request.Interval, null, category: request.Category, malwareFamilyId: null, sourceCountryId: null, ct)).ToList();
        var prevRaw = (await _repo.GetTimelineAnalyticsAsync(comparisonStart, comparisonEnd, request.Interval, null, category: request.Category, malwareFamilyId: null, sourceCountryId: null, ct)).ToList();

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

        // Map previous period buckets for trend direction per bucket
        var prevGrouped = prevRaw
            .GroupBy(r => r.Timestamp)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Count));

        // Optionally annotate per-bucket trend (store in Severity dict placeholder for now if needed later)
        foreach (var bucket in grouped)
        {
            if (prevGrouped.TryGetValue(bucket.Timestamp - window, out int prevCount))
            {
                // store relative diff as pseudo severity metric (negative allowed) under key "_trend"
                double change = prevCount == 0 ? 100 : ((double)bucket.EventCount - prevCount) / prevCount * 100.0;
                bucket.Severity["_trend_pct"] = (int)Math.Round(change); // coarse integer percent
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
