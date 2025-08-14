using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.BulkAssignAsnRegistriesToTenant;

public record BulkAssignAsnRegistriesToTenantCommand : IRequest<Unit>, MeUi.Application.Models.ITenantRequest
{
    public Guid TenantId { get; set; }
    public List<Guid> AsnRegistryIds { get; set; } = new();
    public Guid AssignedByTenantUserId { get; set; }
}
