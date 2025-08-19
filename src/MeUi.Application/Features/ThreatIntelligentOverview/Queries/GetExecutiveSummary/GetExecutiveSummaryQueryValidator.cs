using FluentValidation;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetExecutiveSummary;

public class GetExecutiveSummaryQueryValidator : AbstractValidator<GetExecutiveSummaryQuery>
{
    public GetExecutiveSummaryQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}
