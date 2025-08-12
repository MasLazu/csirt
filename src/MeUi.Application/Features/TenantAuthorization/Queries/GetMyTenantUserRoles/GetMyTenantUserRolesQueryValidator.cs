using FluentValidation;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetMyTenantUserRoles;

public class GetMyTenantUserRolesQueryValidator : AbstractValidator<GetMyTenantUserRolesQuery>
{
    public GetMyTenantUserRolesQueryValidator()
    {
        RuleFor(x => x.TenantUserId)
            .NotEmpty()
            .WithMessage("TenantUserId is required");
    }
}
