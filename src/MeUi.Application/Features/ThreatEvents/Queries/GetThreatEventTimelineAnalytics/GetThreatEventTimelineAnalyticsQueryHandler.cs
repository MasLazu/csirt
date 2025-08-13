using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTimelineAnalytics;

public class GetThreatEventTimelineAnalyticsQueryHandler : IRequestHandler<GetThreatEventTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _analyticsRepository;

    public GetThreatEventTimelineAnalyticsQueryHandler(IThreatEventAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }
    public async Task<ThreatEventTimelineAnalyticsDto> Handle(GetThreatEventTimelineAnalyticsQuery request, CancellationToken ct)
    {
        // Ensure DateTime values are in UTC for PostgreSQL compatibility
        DateTime startTimeUtc = request.StartTime.Kind == DateTimeKind.Utc ? request.StartTime : request.StartTime.ToUniversalTime();
        DateTime endTimeUtc = request.EndTime.Kind == DateTimeKind.Utc ? request.EndTime : request.EndTime.ToUniversalTime();

        // Launch category and malware timeline in parallel
        Guid? tenantId = null;
        var categoryTimelineTask = _analyticsRepository.GetTimelineAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.Interval,
            tenantId,
            category: request.Category,
            malwareFamilyId: request.MalwareFamilyId,
            sourceCountryId: request.SourceCountryId,
            ct);
        var malwareTimelineTask = _analyticsRepository.GetMalwareTimelineAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.Interval,
            tenantId,
            ct);

        await Task.WhenAll(categoryTimelineTask, malwareTimelineTask);
        IEnumerable<TimelineDataPoint> timelineData = await categoryTimelineTask;
        var malwareTimeline = (await malwareTimelineTask)
            .GroupBy(m => m.Timestamp)
            .ToDictionary(g => g.Key, g => g
                .OrderByDescending(x => x.Count)
                .Take(request.TopItemsLimit)
                .ToDictionary(x => x.MalwareFamilyName, x => x.Count));

        // Get summary for total count
        ThreatEventSummary summary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            tenantId,
            ct);

        // Aggregate per time bucket (results currently one row per bucket & category)
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

        // Calculate trends if requested
        TrendAnalysisDto trends = new();
        if (request.IncludeTrends)
        {
            trends = await CalculateTrendAnalysis(request, tenantId, ct);
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
        GetThreatEventTimelineAnalyticsQuery request,
        Guid? tenantId,
        CancellationToken ct)
    {
        TimeSpan timeRange = request.EndTime - request.StartTime;
        DateTime comparisonStart = request.StartTime - timeRange;
        DateTime comparisonEnd = request.StartTime;

        // Use comparative timeline from the new repository
        IEnumerable<ComparativeTimelineDataPoint> comparativeData = await _analyticsRepository.GetComparativeTimelineAsync(
            request.StartTime,
            request.EndTime,
            comparisonStart,
            comparisonEnd,
            request.Interval,
            tenantId,
            ct);

        // Calculate overall percentage change
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
