using FluentValidation;

namespace MeUi.Application.Features.Countries.Queries.GetCountry;

public class GetCountryQueryValidator : AbstractValidator<GetCountryQuery>
{
    public GetCountryQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Country ID is required");
    }
}
