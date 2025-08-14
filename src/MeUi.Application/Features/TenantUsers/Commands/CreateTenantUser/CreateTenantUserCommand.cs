using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;

public record CreateTenantUserCommand : IRequest<Guid>
    , ITenantRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public ICollection<Guid> RoleIds { get; set; } = new List<Guid>();
}