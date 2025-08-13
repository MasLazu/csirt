using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventTimelineAnalytics;

public class GetTenantThreatEventTimelineAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetTenantThreatEventTimelineAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }
    public async Task<ThreatEventTimelineAnalyticsDto> Handle(GetTenantThreatEventTimelineAnalyticsQuery request, CancellationToken ct)
    {
        DateTime startTimeUtc = request.StartTime.Kind == DateTimeKind.Utc ? request.StartTime : request.StartTime.ToUniversalTime();
        DateTime endTimeUtc = request.EndTime.Kind == DateTimeKind.Utc ? request.EndTime : request.EndTime.ToUniversalTime();

        var categoryTimelineTask = _analyticsRepository.GetTimelineAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.Interval,
            request.TenantId,
            category: request.Category,
            malwareFamilyId: request.MalwareFamilyId,
            sourceCountryId: request.SourceCountryId,
            ct);
        var malwareTimelineTask = _analyticsRepository.GetMalwareTimelineAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.Interval,
            request.TenantId,
            ct);

        await Task.WhenAll(categoryTimelineTask, malwareTimelineTask);
        IEnumerable<TimelineDataPoint> timelineData = await categoryTimelineTask;
        var malwareTimeline = (await malwareTimelineTask)
            .GroupBy(m => m.Timestamp)
            .ToDictionary(g => g.Key, g => g
                .OrderByDescending(x => x.Count)
                .Take(request.TopItemsLimit)
                .ToDictionary(x => x.MalwareFamilyName, x => x.Count));

        ThreatEventSummary summary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.TenantId,
            ct);

        var groupedTimeline = timelineData
            .GroupBy(p => p.Timestamp)
            .Select(g => new ThreatEventTimelineDto
            {
                Timestamp = g.Key,
                EventCount = g.Sum(x => x.Count),
                Categories = (request.TopItemsLimit > 0
                    ? g.OrderByDescending(x => x.Count).Take(request.TopItemsLimit)
                    : g)
                    .ToDictionary(x => x.Category, x => x.Count),
                MalwareFamilies = malwareTimeline.TryGetValue(g.Key, out var mDict) ? mDict : new Dictionary<string, int>()
            })
            .OrderBy(t => t.Timestamp)
            .ToList();

        TrendAnalysisDto trends = new();
        if (request.IncludeTrends)
        {
            trends = await CalculateTrendAnalysis(request, ct);
        }

        return new ThreatEventTimelineAnalyticsDto
        {
            Timeline = groupedTimeline,
            TotalEvents = summary.TotalEvents,
            TimeRange = new TimeRangeDto
            {
                Start = request.StartTime,
                End = request.EndTime
            },
            Interval = request.Interval,
            Trends = trends
        };
    }

    private async Task<TrendAnalysisDto> CalculateTrendAnalysis(
        GetTenantThreatEventTimelineAnalyticsQuery request,
        CancellationToken ct)
    {
        TimeSpan timeRange = request.EndTime - request.StartTime;
        DateTime comparisonStart = request.StartTime - timeRange;
        DateTime comparisonEnd = request.StartTime;

        IEnumerable<ComparativeTimelineDataPoint> comparativeData = await _analyticsRepository.GetComparativeTimelineAsync(
            request.StartTime,
            request.EndTime,
            comparisonStart,
            comparisonEnd,
            request.Interval,
            request.TenantId,
            ct);

        int currentPeriodTotal = comparativeData.Sum(cp => cp.Count);
        int previousPeriodTotal = comparativeData.Sum(cp => cp.PreviousPeriodCount);

        double percentageChange = previousPeriodTotal > 0
            ? (double)(currentPeriodTotal - previousPeriodTotal) / previousPeriodTotal * 100
            : 0;

        string direction = percentageChange switch
        {
            > 5 => "increasing",
            < -5 => "decreasing",
            _ => "stable"
        };

        return new TrendAnalysisDto
        {
            PercentageChange = percentageChange,
            Direction = direction,
            ComparisonPeriodStart = comparisonStart,
            ComparisonPeriodEnd = comparisonEnd
        };
    }
}
