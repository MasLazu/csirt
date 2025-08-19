using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetCurrentRiskLevel;

public class GetCurrentRiskLevelQueryValidator : AbstractValidator<GetCurrentRiskLevelQuery>
{
    public GetCurrentRiskLevelQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
    }
}
