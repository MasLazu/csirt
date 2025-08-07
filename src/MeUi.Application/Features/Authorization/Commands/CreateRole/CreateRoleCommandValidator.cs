using FluentValidation;

namespace MeUi.Application.Features.Authorization.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Role code is required.")
            .MaximumLength(50)
            .WithMessage("Role code must not exceed 50 characters.")
            .Matches(@"^[A-Z][A-Z0-9_]*$")
            .WithMessage("Role code must start with a letter and contain only uppercase letters, numbers, and underscores.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .MaximumLength(100)
            .WithMessage("Role name must not exceed 100 characters.")
            .MinimumLength(2)
            .WithMessage("Role name must be at least 2 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("Role description must not exceed 255 characters.");
    }
}