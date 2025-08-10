using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.UpdateRole;

public record UpdateRoleCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}