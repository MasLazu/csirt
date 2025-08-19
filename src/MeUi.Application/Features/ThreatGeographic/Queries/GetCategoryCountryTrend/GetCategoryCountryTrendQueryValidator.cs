using FluentValidation;

namespace MeUi.Application.Features.ThreatGeographic.Queries.GetCategoryCountryTrend;

public class GetCategoryCountryTrendQueryValidator : AbstractValidator<GetCategoryCountryTrendQuery>
{
    public GetCategoryCountryTrendQueryValidator()
    {
        RuleFor(x => x.Start).LessThanOrEqualTo(x => x.End).WithMessage("Start must be before or equal to End");
    }
}
