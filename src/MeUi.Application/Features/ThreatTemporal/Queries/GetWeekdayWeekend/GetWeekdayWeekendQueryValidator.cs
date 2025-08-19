using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeekdayWeekend;

public class GetWeekdayWeekendQueryValidator : AbstractValidator<GetWeekdayWeekendQuery>
{
    public GetWeekdayWeekendQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
