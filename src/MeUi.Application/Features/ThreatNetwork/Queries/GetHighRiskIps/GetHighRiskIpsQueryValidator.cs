using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetHighRiskIps;

public class GetHighRiskIpsQueryValidator : AbstractValidator<GetHighRiskIpsQuery>
{
    public GetHighRiskIpsQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
        RuleFor(x => x.Limit).GreaterThan(0);
    }
}
