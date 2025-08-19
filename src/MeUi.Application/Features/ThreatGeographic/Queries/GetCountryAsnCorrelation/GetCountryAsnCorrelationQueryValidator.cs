using FluentValidation;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCountryAsnCorrelation;

public class GetCountryAsnCorrelationQueryValidator : AbstractValidator<GetCountryAsnCorrelationQuery>
{
    public GetCountryAsnCorrelationQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
        RuleFor(x => x.Limit).GreaterThan(0).WithMessage("Limit must be greater than 0");
    }
}
