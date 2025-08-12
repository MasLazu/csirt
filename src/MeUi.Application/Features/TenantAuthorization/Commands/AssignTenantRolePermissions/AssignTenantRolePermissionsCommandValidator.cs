using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Commands.AssignTenantRolePermissions;

public class AssignTenantRolePermissionsCommandValidator : AbstractValidator<AssignTenantRolePermissionsCommand>
{
    public AssignTenantRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.PermissionIds)
            .NotNull()
            .WithMessage("Permission IDs collection is required");

        RuleForEach(x => x.PermissionIds)
            .NotEmpty()
            .WithMessage("Permission ID cannot be empty");
    }
}