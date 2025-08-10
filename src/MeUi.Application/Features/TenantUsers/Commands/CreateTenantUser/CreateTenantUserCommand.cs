using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.CreateTenantUser;

public record CreateTenantUserCommand : IRequest<Guid>
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid TenantId { get; init; }
    public bool IsTenantAdmin { get; init; } = false;
    public ICollection<Guid> RoleIds { get; init; } = new List<Guid>();
}