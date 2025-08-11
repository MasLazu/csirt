using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.UpdateTenantUser;

public record UpdateTenantUserCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}