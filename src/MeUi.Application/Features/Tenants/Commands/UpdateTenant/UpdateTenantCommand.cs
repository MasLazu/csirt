using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.UpdateTenant;

public record UpdateTenantCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}