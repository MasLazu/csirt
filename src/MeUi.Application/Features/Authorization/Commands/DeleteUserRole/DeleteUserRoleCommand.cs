using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.DeleteUserRole;

public record DeleteUserRoleCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
