using MediatR;
using MeUi.Application.Features.Users.Models;

namespace MeUi.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; init; }
}