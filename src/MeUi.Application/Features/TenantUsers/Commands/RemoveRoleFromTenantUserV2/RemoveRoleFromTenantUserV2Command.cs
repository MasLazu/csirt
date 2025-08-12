using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.RemoveRoleFromTenantUserV2;

public record RemoveRoleFromTenantUserV2Command : IRequest<Unit>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
