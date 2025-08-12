using MeUi.Application.Models;

namespace MeUi.Application.Features.Users.Queries.GetUsersPaginated;

public record GetUsersPaginatedQuery : BasePaginatedQuery<UserDto>
{
    public bool? IsSuspended { get; set; }
}