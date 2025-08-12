using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Commands.CreateTenantRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateTenantRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

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