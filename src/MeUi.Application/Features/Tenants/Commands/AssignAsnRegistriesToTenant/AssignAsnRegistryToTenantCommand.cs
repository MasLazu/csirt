using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.AssignAsnRegistriesToTenant;

public record AssignAsnRegistryToTenantCommand : IRequest<Unit>
{
    public Guid TenantId { get; set; }
    public Guid AsnId { get; set; }
    public Guid AssignedByTenantUserId { get; set; }
}