using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantUserRoles;

public class AssignTenantUserRolesCommandValidator : AbstractValidator<AssignTenantUserRolesCommand>
{
    public AssignTenantUserRolesCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage("Role IDs collection is required");

        RuleForEach(x => x.RoleIds)
            .NotEmpty()
            .WithMessage("Role ID cannot be empty");
    }
}