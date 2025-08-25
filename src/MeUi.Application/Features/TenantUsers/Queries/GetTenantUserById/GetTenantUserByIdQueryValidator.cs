using FluentValidation;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;

public class GetTenantUserByIdQueryValidator : AbstractValidator<GetTenantUserByIdQuery>
{
    public GetTenantUserByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required.");
    }
}
