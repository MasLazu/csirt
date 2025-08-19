using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetTargetedInfrastructure;

public class GetTargetedInfrastructureQueryValidator : AbstractValidator<GetTargetedInfrastructureQuery>
{
    public GetTargetedInfrastructureQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
        RuleFor(x => x.Limit).GreaterThan(0);
    }
}
