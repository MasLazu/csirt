using FluentValidation;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryRankings;

public class GetCountryRankingsQueryValidator : AbstractValidator<GetCountryRankingsQuery>
{
    public GetCountryRankingsQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
        RuleFor(x => x.Limit).GreaterThan(0).WithMessage("Limit must be greater than 0");
    }
}
