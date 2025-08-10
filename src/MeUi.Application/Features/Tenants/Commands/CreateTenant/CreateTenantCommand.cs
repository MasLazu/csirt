using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string ContactPhone { get; init; } = string.Empty;
}