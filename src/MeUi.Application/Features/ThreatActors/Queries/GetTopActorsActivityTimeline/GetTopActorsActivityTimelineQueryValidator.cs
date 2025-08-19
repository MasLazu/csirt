using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetTopActorsActivityTimeline;

public class GetTopActorsActivityTimelineQueryValidator : AbstractValidator<GetTopActorsActivityTimelineQuery>
{
    public GetTopActorsActivityTimelineQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End).WithMessage("Start must be before End.");
        RuleFor(x => x.Interval).GreaterThan(TimeSpan.Zero).WithMessage("Interval must be > 0.");
    }
}
