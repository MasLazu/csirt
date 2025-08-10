using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.AssignRoleToTenantUser;

public record AssignRoleToTenantUserCommand : IRequest<Unit>
{
    public Guid TenantUserId { get; init; }
    public Guid RoleId { get; init; }
}