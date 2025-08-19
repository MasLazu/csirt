using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.Get24HourAttackPattern;

public class Get24HourAttackPatternQueryValidator : AbstractValidator<Get24HourAttackPatternQuery>
{
    public Get24HourAttackPatternQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
