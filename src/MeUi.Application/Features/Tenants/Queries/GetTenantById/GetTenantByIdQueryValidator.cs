using FluentValidation;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantById;

public sealed class GetTenantByIdQueryValidator : AbstractValidator<GetTenantByIdQuery>
{
    public GetTenantByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Tenant ID is required");
    }
}
