using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.UpdateRole;

public record UpdateRoleCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<Guid> PermissionIds { get; set; } = [];
}