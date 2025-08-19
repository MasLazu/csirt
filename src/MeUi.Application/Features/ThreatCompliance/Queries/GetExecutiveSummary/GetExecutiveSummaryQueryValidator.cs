using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetExecutiveSummary;

public class GetExecutiveSummaryQueryValidator : AbstractValidator<GetExecutiveSummaryQuery>
{
    public GetExecutiveSummaryQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End)
            .WithMessage("Start must be before or equal to End");
    }
}
