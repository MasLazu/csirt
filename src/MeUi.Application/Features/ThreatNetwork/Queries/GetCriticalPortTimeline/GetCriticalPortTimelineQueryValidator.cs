using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetCriticalPortTimeline;

public class GetCriticalPortTimelineQueryValidator : AbstractValidator<GetCriticalPortTimelineQuery>
{
    public GetCriticalPortTimelineQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
