using FluentValidation;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAttackTrends;

public class GetCountryAttackTrendsQueryValidator : AbstractValidator<GetCountryAttackTrendsQuery>
{
    public GetCountryAttackTrendsQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End)
            .WithMessage("Start must be before or equal to End");
    }
}
