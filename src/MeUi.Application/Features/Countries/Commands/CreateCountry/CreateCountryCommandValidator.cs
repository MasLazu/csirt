using FluentValidation;

namespace MeUi.Application.Features.Countries.Commands.CreateCountry;

public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Country code is required")
            .Length(2, 3)
            .WithMessage("Country code must be 2-3 characters")
            .Matches("^[A-Z]+$")
            .WithMessage("Country code must contain only uppercase letters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Country name is required")
            .Length(2, 100)
            .WithMessage("Country name must be between 2 and 100 characters");
    }
}
