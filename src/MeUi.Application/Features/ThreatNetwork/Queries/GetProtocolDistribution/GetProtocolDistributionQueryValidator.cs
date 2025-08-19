using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetProtocolDistribution;

public class GetProtocolDistributionQueryValidator : AbstractValidator<GetProtocolDistributionQuery>
{
    public GetProtocolDistributionQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
    }
}
