using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetTimeOfDayPatterns;

public class GetTimeOfDayPatternsQueryValidator : AbstractValidator<GetTimeOfDayPatternsQuery>
{
    public GetTimeOfDayPatternsQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
