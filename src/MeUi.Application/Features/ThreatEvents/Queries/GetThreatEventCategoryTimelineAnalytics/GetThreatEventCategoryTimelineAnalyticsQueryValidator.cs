using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventCategoryTimelineAnalytics;

public class GetThreatEventCategoryTimelineAnalyticsQueryValidator : AbstractValidator<GetThreatEventCategoryTimelineAnalyticsQuery>
{
    private static readonly HashSet<string> Allowed = new(["5m", "15m", "1h", "6h", "1d", "1w"], StringComparer.OrdinalIgnoreCase);
    public GetThreatEventCategoryTimelineAnalyticsQueryValidator()
    {
        RuleFor(x => x.Interval).Must(v => Allowed.Contains(v));
        RuleFor(x => x.Category).MaximumLength(128);
        RuleFor(x => x).Must(HaveValidRange).WithMessage("Invalid time range");
    }
    private static bool HaveValidRange(GetThreatEventCategoryTimelineAnalyticsQuery q)
    {
        if (q.StartTime.HasValue && q.EndTime.HasValue)
        {
            if (q.StartTime >= q.EndTime) return false;
            return (q.EndTime.Value - q.StartTime.Value).TotalDays <= 180;
        }
        return true;
    }
}
