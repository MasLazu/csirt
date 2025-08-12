using FluentValidation;

namespace MeUi.Application.Features.AsnRegistries.Commands.UpdateAsnRegistry;

public class UpdateAsnRegistryCommandValidator : AbstractValidator<UpdateAsnRegistryCommand>
{
    public UpdateAsnRegistryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ASN Registry ID is required");

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
