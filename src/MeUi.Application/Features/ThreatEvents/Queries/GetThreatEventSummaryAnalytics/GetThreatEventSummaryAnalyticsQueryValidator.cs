using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventSummaryAnalytics;

public class GetThreatEventSummaryAnalyticsQueryValidator : AbstractValidator<GetThreatEventSummaryAnalyticsQuery>
{
    public GetThreatEventSummaryAnalyticsQueryValidator()
    {
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.TopItemsLimit)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Top items limit must be between 1 and 50");

        // Ensure time range is reasonable (max 1 year for summary)
        RuleFor(x => x)
            .Must(HaveReasonableTimeRange)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Time range cannot exceed 1 year for summary analytics");
    }

    private static bool HaveReasonableTimeRange(GetThreatEventSummaryAnalyticsQuery query)
    {
        if (!query.StartTime.HasValue || !query.EndTime.HasValue)
            return true;

        TimeSpan timeRange = query.EndTime.Value - query.StartTime.Value;
        return timeRange <= TimeSpan.FromDays(365);
    }
}
