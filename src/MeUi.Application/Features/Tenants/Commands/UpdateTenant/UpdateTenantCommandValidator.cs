using FluentValidation;

namespace MeUi.Application.Features.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tenant name is required")
            .MaximumLength(100)
            .WithMessage("Tenant name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("Contact email is required")
            .EmailAddress()
            .WithMessage("Contact email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Contact email cannot exceed 255 characters");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20)
            .WithMessage("Contact phone cannot exceed 20 characters");
    }
}