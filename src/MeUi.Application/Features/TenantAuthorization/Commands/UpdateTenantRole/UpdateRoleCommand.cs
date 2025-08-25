using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;

public record UpdateTenantRoleCommand : IRequest<Guid>, ITenantRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<Guid> PermissionIds { get; set; } = Enumerable.Empty<Guid>();
}