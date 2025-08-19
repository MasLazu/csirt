using FluentValidation;

namespace MeUi.Application.Features.ThreatActors.Queries.GetActorDistributionByCountry;

public class GetActorDistributionByCountryQueryValidator : AbstractValidator<GetActorDistributionByCountryQuery>
{
    public GetActorDistributionByCountryQueryValidator()
    {
        RuleFor(x => x.Start)
            .LessThan(x => x.End)
            .WithMessage("Start must be before End.");
    }
}
