using FluentValidation;

namespace MeUi.Application.Features.AsnRegistries.Commands.CreateAsnRegistry;

public class CreateAsnRegistryCommandValidator : AbstractValidator<CreateAsnRegistryCommand>
{
    public CreateAsnRegistryCommandValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("ASN number is required")
            .MaximumLength(20)
            .WithMessage("ASN number cannot exceed 20 characters")
            .Matches(@"^AS\d+$|^\d+$")
            .WithMessage("ASN number must be a valid format (e.g., 'AS64512' or '64512')");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("ASN description is required")
            .MaximumLength(255)
            .WithMessage("ASN description cannot exceed 255 characters");
    }
}
