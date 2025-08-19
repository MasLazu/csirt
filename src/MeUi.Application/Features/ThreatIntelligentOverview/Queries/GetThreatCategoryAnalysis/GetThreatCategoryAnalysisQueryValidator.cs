using FluentValidation;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;

public class GetThreatCategoryAnalysisQueryValidator : AbstractValidator<GetThreatCategoryAnalysisQuery>
{
    public GetThreatCategoryAnalysisQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("EndTime must be after StartTime.");
    }
}
