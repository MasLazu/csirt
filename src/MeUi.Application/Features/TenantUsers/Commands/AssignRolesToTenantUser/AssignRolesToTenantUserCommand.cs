using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRolesToTenantUser;

public record AssignRolesToTenantUserCommand : IRequest<IEnumerable<Guid>>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable<Guid> RoleIds { get; set; } = [];
}
