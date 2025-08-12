using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.CreateTenant;

public record CreateTenantCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}