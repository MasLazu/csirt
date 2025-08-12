using MediatR;

namespace MeUi.Application.Features.TenantAuthorization.Commands.UpdateTenantRole;

public record UpdateTenantRoleCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}