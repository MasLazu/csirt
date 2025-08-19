using FluentValidation;

namespace MeUi.Application.Features.ThreatNetwork.Queries.GetMostTargetedPorts;

public class GetMostTargetedPortsQueryValidator : AbstractValidator<GetMostTargetedPortsQuery>
{
    public GetMostTargetedPortsQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End);
        RuleFor(x => x.Limit).GreaterThan(0).WithMessage("Limit must be greater than zero");
    }
}
