using FluentValidation;

namespace MeUi.Application.Features.TenantThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;

public class GetTenantThreatCategoryAnalysisQueryValidator : AbstractValidator<GetTenantThreatCategoryAnalysisQuery>
{
    public GetTenantThreatCategoryAnalysisQueryValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}