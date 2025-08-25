using FluentValidation;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public class UpdateTenantUserCommandValidator : AbstractValidator<UpdateTenantUserCommand>
{
    public UpdateTenantUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(255)
            .WithMessage("Name must not exceed 255 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters.");

        RuleFor(x => x.Username)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Username))
            .WithMessage("Username must not exceed 100 characters.")
            .Matches("^[a-zA-Z0-9_.-]*$")
            .When(x => !string.IsNullOrEmpty(x.Username))
            .WithMessage("Username can only contain letters, numbers, dots, hyphens, and underscores.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Username))
            .WithMessage("Either Email or Username must be provided.");
    }
}
