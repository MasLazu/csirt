using MediatR;
using MeUi.Application.Commands;

namespace MeUi.Application.Features.TenantAuthorization.Commands.DeleteTenantRole;

public record DeleteTenantRoleCommand : BaseTenantCommand, IRequest<Guid>
{
    public Guid Id { get; set; }
}