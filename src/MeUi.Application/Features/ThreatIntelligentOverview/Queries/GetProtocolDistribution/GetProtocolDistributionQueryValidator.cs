using FluentValidation;

namespace MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetProtocolDistribution;

public class GetProtocolDistributionQueryValidator : AbstractValidator<GetProtocolDistributionQuery>
{
    public GetProtocolDistributionQueryValidator()
    {
        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("StartTime must be before EndTime.");
    }
}
