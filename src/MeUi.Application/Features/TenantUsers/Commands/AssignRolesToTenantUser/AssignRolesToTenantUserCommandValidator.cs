using FluentValidation;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRolesToTenantUser;

public class AssignRolesToTenantUserCommandValidator : AbstractValidator<AssignRolesToTenantUserCommand>
{
    public AssignRolesToTenantUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage("RoleIds is required.");
    }
}
