using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.AssignAsnRegistriesToTenant;

public record AssignAsnRegistryToTenantCommand(
    Guid TenantId,
    Guid AsnId,
    Guid AssignedByTenantUserId
) : IRequest<Unit>;