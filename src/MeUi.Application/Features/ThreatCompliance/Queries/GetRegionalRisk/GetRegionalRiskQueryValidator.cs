using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetRegionalRisk;

public class GetRegionalRiskQueryValidator : AbstractValidator<GetRegionalRiskQuery>
{
    public GetRegionalRiskQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
        RuleFor(x => x.Limit).InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100");
    }
}
