using FluentValidation;

namespace MeUi.Application.Features.TenantThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTenantTopActorsActivityTimelineQueryValidator : AbstractValidator<GetTenantTopActorsActivityTimelineQuery>
{
    public GetTenantTopActorsActivityTimelineQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");

        RuleFor(x => x.Interval)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Interval must be greater than zero.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Limit must be between 1 and 100.");
    }
}