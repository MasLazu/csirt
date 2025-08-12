using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.CreateUserRole;

public record CreateUserRoleCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
