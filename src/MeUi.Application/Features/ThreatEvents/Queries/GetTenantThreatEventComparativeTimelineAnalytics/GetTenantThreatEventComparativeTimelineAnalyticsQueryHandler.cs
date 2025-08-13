using MediatR;
using MeUi.Application.Interfaces;
using MeUi.Application.Models.Analytics;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetTenantThreatEventComparativeTimelineAnalytics;

public class GetTenantThreatEventComparativeTimelineAnalyticsQueryHandler : IRequestHandler<GetTenantThreatEventComparativeTimelineAnalyticsQuery, ThreatEventTimelineAnalyticsDto>
{
    private readonly IThreatEventAnalyticsRepository _repo;

    public GetTenantThreatEventComparativeTimelineAnalyticsQueryHandler(IThreatEventAnalyticsRepository repo)
    {
        _repo = repo;
    }

    public async Task<ThreatEventTimelineAnalyticsDto> Handle(GetTenantThreatEventComparativeTimelineAnalyticsQuery request, CancellationToken ct)
    {
        DateTime currentEnd = request.CurrentEnd ?? DateTime.UtcNow;
        DateTime currentStart = request.CurrentStart ?? currentEnd.AddDays(-7);
        var window = currentEnd - currentStart;
        DateTime previousEnd = request.PreviousEnd ?? currentStart;
        DateTime previousStart = request.PreviousStart ?? previousEnd - window;

        var current = (await _repo.GetComparativeTimelineAsync(currentStart, currentEnd, previousStart, previousEnd, request.Interval, request.TenantId, ct)).ToList();

        var timelineDtos = current.Select(c => new ThreatEventTimelineDto
        {
            Timestamp = c.Timestamp,
            EventCount = c.Count,
            Categories = new Dictionary<string, int> { { c.Category, c.Count } },
            MalwareFamilies = new(),
            Severity = new()
        }).ToList();

        double totalCurrent = current.Sum(c => (double)c.Count);
        double totalPrev = current.Sum(c => (double)c.PreviousPeriodCount);
        double pctChange = totalPrev == 0 ? 100 : ((totalCurrent - totalPrev) / totalPrev) * 100.0;
        string direction = totalCurrent > totalPrev ? "increasing" : totalCurrent < totalPrev ? "decreasing" : "stable";

        return new ThreatEventTimelineAnalyticsDto
        {
            Timeline = timelineDtos,
            TotalEvents = (int)totalCurrent,
            Interval = request.Interval,
            TimeRange = new TimeRangeDto { Start = currentStart, End = currentEnd },
            Trends = new TrendAnalysisDto
            {
                PercentageChange = pctChange,
                Direction = direction,
                ComparisonPeriodStart = previousStart,
                ComparisonPeriodEnd = previousEnd
            }
        };
    }
}
