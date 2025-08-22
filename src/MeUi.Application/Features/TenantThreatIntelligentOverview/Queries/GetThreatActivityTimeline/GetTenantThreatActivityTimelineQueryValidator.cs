using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetTenantThreatActivityTimelineQueryValidator : AbstractValidator<GetTenantThreatActivityTimelineQuery>
{
    public GetTenantThreatActivityTimelineQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.Interval)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Interval must be greater than zero.");
    }
}