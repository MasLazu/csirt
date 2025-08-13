using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventComparativeTimelineAnalytics;

public class GetThreatEventComparativeTimelineAnalyticsQueryValidator : AbstractValidator<GetThreatEventComparativeTimelineAnalyticsQuery>
{
    private static readonly HashSet<string> Allowed = new(["5m", "15m", "1h", "6h", "1d", "1w"], StringComparer.OrdinalIgnoreCase);

    public GetThreatEventComparativeTimelineAnalyticsQueryValidator()
    {
        RuleFor(x => x.Interval).Must(v => Allowed.Contains(v)).WithMessage("Invalid interval");
        RuleFor(x => x).Must(HaveValidRanges).WithMessage("Invalid time ranges");
    }

    private static bool HaveValidRanges(GetThreatEventComparativeTimelineAnalyticsQuery q)
    {
        if (q.CurrentStart.HasValue && q.CurrentEnd.HasValue && q.PreviousStart.HasValue && q.PreviousEnd.HasValue)
        {
            if (q.CurrentStart >= q.CurrentEnd) return false;
            if (q.PreviousStart >= q.PreviousEnd) return false;
            var cur = q.CurrentEnd.Value - q.CurrentStart.Value;
            var prev = q.PreviousEnd.Value - q.PreviousStart.Value;
            if (Math.Abs((cur - prev).TotalMinutes) > 1) return false; // enforce equal windows
            return cur.TotalDays <= 180; // cap
        }
        return true;
    }
}
