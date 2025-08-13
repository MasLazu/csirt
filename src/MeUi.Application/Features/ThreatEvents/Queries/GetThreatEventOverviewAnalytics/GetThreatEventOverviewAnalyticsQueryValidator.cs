using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventOverviewAnalytics;

public class GetThreatEventOverviewAnalyticsQueryValidator : AbstractValidator<GetThreatEventOverviewAnalyticsQuery>
{
    public GetThreatEventOverviewAnalyticsQueryValidator()
    {
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.TopItemsLimit)
            .GreaterThan(0)
            .LessThanOrEqualTo(25)
            .WithMessage("Top items limit must be between 1 and 25");

        RuleFor(x => x)
            .Must(HaveReasonableTimeRange)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Time range cannot exceed 180 days for overview analytics");
    }

    private static bool HaveReasonableTimeRange(GetThreatEventOverviewAnalyticsQuery query)
    {
        if (!query.StartTime.HasValue || !query.EndTime.HasValue)
            return true;
        return (query.EndTime.Value - query.StartTime.Value) <= TimeSpan.FromDays(180);
    }
}
