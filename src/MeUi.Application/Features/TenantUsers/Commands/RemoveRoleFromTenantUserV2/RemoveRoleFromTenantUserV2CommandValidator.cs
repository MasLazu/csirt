using FluentValidation;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUserV2;

public class RemoveRoleFromTenantUserV2CommandValidator : AbstractValidator<RemoveRoleFromTenantUserV2Command>
{
    public RemoveRoleFromTenantUserV2CommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("RoleId is required.");
    }
}
