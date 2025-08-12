using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;

public class GetTenantsPaginatedQueryValidator : AbstractValidator<GetTenantsPaginatedQuery>
{
    public GetTenantsPaginatedQueryValidator()
    {
        // Include base validation rules
        Include(new BasePaginatedQueryValidator<TenantDto>());

        // Additional validations specific to tenant queries can be added here
    }
}
