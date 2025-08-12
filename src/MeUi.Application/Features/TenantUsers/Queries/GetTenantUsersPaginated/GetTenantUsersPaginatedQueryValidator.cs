using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Queries.GetTenantUsersPaginated;

public class GetTenantUsersPaginatedQueryValidator : AbstractValidator<GetTenantUsersPaginatedQuery>
{
    public GetTenantUsersPaginatedQueryValidator()
    {
        // Include base validation rules
        Include(new BasePaginatedQueryValidator<TenantUserDto>());

        // Add specific validation for this query
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required.");
    }
}
