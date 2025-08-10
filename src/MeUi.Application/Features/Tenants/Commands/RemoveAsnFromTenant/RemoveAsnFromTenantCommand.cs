using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.RemoveAsnFromTenant;

public record RemoveAsnFromTenantCommand(
    Guid TenantId,
    Guid AsnId
) : IRequest<Unit>;