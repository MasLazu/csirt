using FluentValidation;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Users.Queries.GetUsersPaginated;

public class GetUsersPaginatedQueryValidator : AbstractValidator<GetUsersPaginatedQuery>
{
    public GetUsersPaginatedQueryValidator()
    {
        // Include base validation rules
        Include(new BasePaginatedQueryValidator<UserDto>());

        // Additional validations specific to user queries can be added here
    }
}