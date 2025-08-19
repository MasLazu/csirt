using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorAsn;

public class GetActorAsnQueryValidator : AbstractValidator<GetActorAsnQuery>
{
    public GetActorAsnQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}
