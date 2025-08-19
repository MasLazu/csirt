using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorEvolution;

public class GetActorEvolutionQueryValidator : AbstractValidator<GetActorEvolutionQuery>
{
    public GetActorEvolutionQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End).WithMessage("Start must be before End.");
    }
}
