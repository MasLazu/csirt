using FluentValidation;

namespace MeUi.Application.Features.ThreatEvents.Queries.GetThreatEventTopPortsAnalytics;

public class GetThreatEventTopPortsAnalyticsValidator : AbstractValidator<GetThreatEventTopPortsAnalyticsQuery>
{
    public GetThreatEventTopPortsAnalyticsValidator()
    {
        RuleFor(x => x.Top).GreaterThan(0).LessThanOrEqualTo(200);
        RuleFor(x => x.EndTime)
            .Must((query, end) => end == null || query.StartTime == null || end > query.StartTime)
            .WithMessage("EndTime must be greater than StartTime");
    }
}
