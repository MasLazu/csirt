using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventProtocolDistributionAnalytics;

public class GetThreatEventProtocolDistributionAnalyticsQueryValidator : AbstractValidator<GetThreatEventProtocolDistributionAnalyticsQuery>
{
    public GetThreatEventProtocolDistributionAnalyticsQueryValidator()
    {
        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("End time must be after start time");

        RuleFor(x => x)
            .Must(HaveReasonableTimeRange)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Time range cannot exceed 180 days for protocol distribution");
    }

    private static bool HaveReasonableTimeRange(GetThreatEventProtocolDistributionAnalyticsQuery q)
        => !q.StartTime.HasValue || !q.EndTime.HasValue || (q.EndTime - q.StartTime) <= TimeSpan.FromDays(180);
}
