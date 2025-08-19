using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetMonthlyGrowth;

public class GetMonthlyGrowthQueryValidator : AbstractValidator<GetMonthlyGrowthQuery>
{
    public GetMonthlyGrowthQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
