using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorSimilarity;

public class GetActorSimilarityQueryValidator : AbstractValidator<GetActorSimilarityQuery>
{
    public GetActorSimilarityQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End).WithMessage("Start must be before End.");
    }
}
