using FluentValidation;

namespace MeUi.Application.Features.Authorization.Commands.AssignUserRoles;

public class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
{
    public AssignUserRolesCommandValidator()
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