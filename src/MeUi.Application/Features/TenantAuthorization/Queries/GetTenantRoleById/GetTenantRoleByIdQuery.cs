using MediatR;
using MeUi.Application.Commands;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantRoleById;

public record GetTenantRoleByIdQuery : BaseTenantCommand, IRequest<RoleDto>
{
    public Guid Id { get; set; }
}
