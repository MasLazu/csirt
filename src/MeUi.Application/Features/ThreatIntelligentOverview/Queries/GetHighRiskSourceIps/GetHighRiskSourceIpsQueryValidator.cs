using FluentValidation;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetHighRiskSourceIps;

public class GetHighRiskSourceIpsQueryValidator : AbstractValidator<GetHighRiskSourceIpsQuery>
{
    public GetHighRiskSourceIpsQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("Limit must be between 1 and 50.");
    }
}
