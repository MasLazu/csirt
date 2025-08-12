using FluentValidation;

namespace MeUi.Application.Features.Countries.Commands.UpdateCountry;

public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Country ID is required");

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
