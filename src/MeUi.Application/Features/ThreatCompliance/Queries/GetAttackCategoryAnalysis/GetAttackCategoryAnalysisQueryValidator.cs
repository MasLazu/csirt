using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetAttackCategoryAnalysis;

public class GetAttackCategoryAnalysisQueryValidator : AbstractValidator<GetAttackCategoryAnalysisQuery>
{
    public GetAttackCategoryAnalysisQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
        RuleFor(x => x.Limit).InclusiveBetween(1, 200).WithMessage("Limit must be between 1 and 200");
    }
}
