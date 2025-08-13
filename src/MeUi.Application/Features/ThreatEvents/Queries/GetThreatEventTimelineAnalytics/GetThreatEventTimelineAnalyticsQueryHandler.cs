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

        // Get timeline data using the dedicated analytics repository
        IEnumerable<TimelineDataPoint> timelineData = await _analyticsRepository.GetTimelineAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            request.Interval,
            tenantId: null, // TODO: Extract from claims/context
            category: request.Category,
            malwareFamilyId: request.MalwareFamilyId,
            sourceCountryId: request.SourceCountryId,
            ct);

        // Get summary for total count
        ThreatEventSummary summary = await _analyticsRepository.GetSummaryAnalyticsAsync(
            startTimeUtc,
            endTimeUtc,
            tenantId: null, // TODO: Extract from claims/context
            ct);

        // Convert TimelineDataPoint to ThreatEventTimelineDto
        var timeline = timelineData.Select(point => new ThreatEventTimelineDto
        {
            Timestamp = point.Timestamp,
            EventCount = point.Count,
            Categories = new Dictionary<string, int> { { point.Category, point.Count } },
            MalwareFamilies = new Dictionary<string, int>() // Will be populated below
        }).ToList();

        // Get additional breakdowns if needed
        if (request.TopItemsLimit > 0)
        {
            // Get category analytics for the timeline
            IEnumerable<CategoryAnalytics> categoryAnalytics = await _analyticsRepository.GetTopCategoriesAsync(
                request.StartTime,
                request.EndTime,
                request.TopItemsLimit,
                tenantId: null,
                ct);

            // Get malware family analytics
            IEnumerable<MalwareFamilyAnalytics> malwareFamilyAnalytics = await _analyticsRepository.GetMalwareFamilyAnalyticsAsync(
                request.StartTime,
                request.EndTime,
                request.TopItemsLimit,
                tenantId: null,
                ct);

            // Enhance timeline with category/malware breakdowns
            foreach (ThreatEventTimelineDto timelineDto in timeline)
            {
                // Update categories
                timelineDto.Categories = categoryAnalytics
                    .ToDictionary(ca => ca.Category, ca => ca.Count);

                // Update malware families
                timelineDto.MalwareFamilies = malwareFamilyAnalytics
                    .ToDictionary(mfa => mfa.FamilyName, mfa => mfa.Count);
            }
        }

        // Calculate trends if requested
        TrendAnalysisDto trends = new();
        if (request.IncludeTrends)
        {
            trends = await CalculateTrendAnalysis(request, ct);
        }

        return new ThreatEventTimelineAnalyticsDto
        {
            Timeline = timeline.ToList(),
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
            tenantId: null, // TODO: Extract from claims/context
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
