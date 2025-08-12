using FluentValidation;

namespace MeUi.Application.Features.Authorization.Commands.DeleteUserRole;

public class DeleteUserRoleCommandValidator : AbstractValidator<DeleteUserRoleCommand>
{
    public DeleteUserRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("UserRole ID is required");
    }
}
