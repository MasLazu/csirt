using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public record UpdateTenantUserCommand : IRequest<Unit>
    , ITenantRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IEnumerable<Guid> RoleIds { get; set; } = [];
}