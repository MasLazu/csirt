using FluentValidation;

namespace MeUi.Application.Features.TenantAuthentication.Queries.GetTenantUserLoginMethods;

public class GetTenantUserLoginMethodsQueryValidator : AbstractValidator<GetTenantUserLoginMethodsQuery>
{
    public GetTenantUserLoginMethodsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}