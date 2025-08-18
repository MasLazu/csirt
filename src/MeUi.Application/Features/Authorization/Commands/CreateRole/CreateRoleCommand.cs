using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.CreateRole;

public record CreateRoleCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<Guid> PermissionIds { get; set; } = [];
}