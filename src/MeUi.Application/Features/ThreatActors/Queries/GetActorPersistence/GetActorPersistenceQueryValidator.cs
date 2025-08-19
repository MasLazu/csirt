using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorPersistence;

public class GetActorPersistenceQueryValidator : AbstractValidator<GetActorPersistenceQuery>
{
    public GetActorPersistenceQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End).WithMessage("Start must be before End.");
    }
}
