using MediatR;
using MeUi.Application.Commands;

namespace MeUi.Application.Features.TenantAuthorization.Commands.CreateTenantRole;

public record CreateTenantRoleCommand : BaseTenantCommand, IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}