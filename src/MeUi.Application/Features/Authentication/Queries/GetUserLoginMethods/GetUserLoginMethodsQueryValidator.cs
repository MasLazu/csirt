using FluentValidation;

namespace MeUi.Application.Features.Authentication.Queries.GetUserLoginMethods;

public class GetUserLoginMethodsQueryValidator : AbstractValidator<GetUserLoginMethodsQuery>
{
    public GetUserLoginMethodsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}