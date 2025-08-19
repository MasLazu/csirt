using FluentValidation;

namespace MeUi.Application.Features.ThreatCompliance.Queries.GetComplianceScore;

public class GetComplianceScoreQueryValidator : AbstractValidator<GetComplianceScoreQuery>
{
    public GetComplianceScoreQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThanOrEqualTo(x => x.End)
            .WithMessage("Start must be before or equal to End");
    }
}
