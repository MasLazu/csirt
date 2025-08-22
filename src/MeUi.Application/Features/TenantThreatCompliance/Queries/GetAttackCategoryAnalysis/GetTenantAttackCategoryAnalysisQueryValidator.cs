using FluentValidation;

namespace MeUi.Application.Features.TenantThreatCompliance.Queries.GetAttackCategoryAnalysis;

public class GetTenantAttackCategoryAnalysisQueryValidator : AbstractValidator<GetTenantAttackCategoryAnalysisQuery>
{
    public GetTenantAttackCategoryAnalysisQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Limit must be between 1 and 100.");
    }
}