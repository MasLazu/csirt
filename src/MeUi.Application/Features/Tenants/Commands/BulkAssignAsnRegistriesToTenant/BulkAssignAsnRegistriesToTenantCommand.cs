using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.BulkAssignAsnRegistriesToTenant;

public record BulkAssignAsnRegistriesToTenantCommand(
    Guid TenantId,
    List<Guid> AsnRegistryIds,
    Guid AssignedByTenantUserId
) : IRequest<Unit>;
