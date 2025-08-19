using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorTtp;

public class GetActorTtpQueryValidator : AbstractValidator<GetActorTtpQuery>
{
    public GetActorTtpQueryValidator()
    {
        RuleFor(x => x.Start).LessThan(x => x.End).WithMessage("Start must be before End.");
    }
}
