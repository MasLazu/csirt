using MediatR;

namespace MeUi.Application.Features.Authorization.Commands.CreateRole;

public record CreateRoleCommand : IRequest<Guid>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}