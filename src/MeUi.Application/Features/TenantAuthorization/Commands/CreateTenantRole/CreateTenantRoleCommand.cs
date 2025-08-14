using MediatR;
using MeUi.Application.Commands;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Commands.CreateTenantRole;

public record CreateTenantRoleCommand : BaseTenantCommand, IRequest<Guid>, ITenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}