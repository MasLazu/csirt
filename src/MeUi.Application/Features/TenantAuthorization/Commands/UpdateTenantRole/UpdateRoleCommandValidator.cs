using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateTenantRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Role name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .WithMessage("Role description must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}