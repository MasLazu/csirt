using MediatR;
using MeUi.Application.Features.Users.Models;

namespace MeUi.Application.Features.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery : IRequest<UserDto?>
{
    public string Email { get; init; } = string.Empty;
}