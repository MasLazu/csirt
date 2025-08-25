using FluentValidation;

namespace MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;

public class DeleteTenantUserCommandValidator : AbstractValidator<DeleteTenantUserCommand>
{
    public DeleteTenantUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User Id is required.");
    }
}
