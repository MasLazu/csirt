using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Commands.DeleteTenantRole;

public class DeleteTenantRoleCommandValidator : AbstractValidator<DeleteTenantRoleCommand>
{
    public DeleteTenantRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Role ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");
    }
}