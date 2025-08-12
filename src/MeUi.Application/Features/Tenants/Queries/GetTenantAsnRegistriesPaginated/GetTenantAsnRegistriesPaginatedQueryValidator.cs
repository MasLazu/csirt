using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistriesPaginated;

public class GetTenantAsnRegistriesPaginatedQueryValidator : AbstractValidator<GetTenantAsnRegistriesPaginatedQuery>
{
    public GetTenantAsnRegistriesPaginatedQueryValidator()
    {
        Include(new BasePaginatedQueryValidator<AsnRegistryDto>());

        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");
    }
}
