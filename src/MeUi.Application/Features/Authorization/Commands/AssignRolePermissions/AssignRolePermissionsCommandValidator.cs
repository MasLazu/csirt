using FluentValidation;

namespace MeUi.Application.Features.Authorization.Commands.AssignRolePermissions;

public class AssignRolePermissionsCommandValidator : AbstractValidator<AssignRolePermissionsCommand>
{
    public AssignRolePermissionsCommandValidator()
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