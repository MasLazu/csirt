using FluentValidation;

namespace MeUi.Application.Features.Authorization.Commands.PutUserRoles;

public class PutUserRolesCommandValidator : AbstractValidator<PutUserRolesCommand>
{
    public PutUserRolesCommandValidator()
    {
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
