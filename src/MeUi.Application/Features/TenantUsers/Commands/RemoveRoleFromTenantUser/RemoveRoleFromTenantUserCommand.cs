using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUser;

public record RemoveRoleFromTenantUserCommand : IRequest<Unit>
{
    public Guid TenantUserId { get; init; }
    public Guid RoleId { get; init; }
}