using FluentValidation;

namespace MeUi.Application.Features.ThreatTemporal.Queries.GetWeeklyAttackDistribution;

public class GetWeeklyAttackDistributionQueryValidator : AbstractValidator<GetWeeklyAttackDistributionQuery>
{
    public GetWeeklyAttackDistributionQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
