using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Users.Queries.GetUserByEmail;

public record GetUserByEmailQuery : IRequest<UserDto?>
{
    public string Email { get; set; } = string.Empty;
}