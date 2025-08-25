using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRolesPaginated;

public class GetTenantRolesPaginatedQueryValidator : AbstractValidator<GetTenantRolesPaginatedQuery>
{
    public GetTenantRolesPaginatedQueryValidator()
    {
        // Include base validation rules
        Include(new BasePaginatedQueryValidator<TenantRoleDto>());

        // Add specific validation for this query
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required.");
    }
}
