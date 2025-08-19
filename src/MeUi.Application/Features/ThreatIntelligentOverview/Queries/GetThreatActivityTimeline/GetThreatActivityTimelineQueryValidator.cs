using FluentValidation;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatActivityTimeline;

public class GetThreatActivityTimelineQueryValidator : AbstractValidator<GetThreatActivityTimelineQuery>
{
    public GetThreatActivityTimelineQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.Interval)
            .Must(interval => interval.TotalMinutes >= 1)
            .WithMessage("Interval must be at least 1 minute.");
    }
}
