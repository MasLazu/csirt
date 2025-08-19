using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorProfiles;

public class GetActorProfilesQueryValidator : AbstractValidator<GetActorProfilesQuery>
{
    public GetActorProfilesQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}
