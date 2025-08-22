using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Commands.AssignAsnRegistriesToTenant;

public record AssignAsnRegistryToTenantCommand : IRequest<Unit>, ITenantRequest
{
    public Guid TenantId { get; set; }
    public Guid AsnId { get; set; }
}