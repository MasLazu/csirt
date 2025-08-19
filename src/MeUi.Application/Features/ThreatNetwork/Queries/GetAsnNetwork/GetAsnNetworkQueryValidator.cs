using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetAsnNetwork;

public class GetAsnNetworkQueryValidator : AbstractValidator<GetAsnNetworkQuery>
{
    public GetAsnNetworkQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
        RuleFor(x => x.Limit).GreaterThan(0);
    }
}
