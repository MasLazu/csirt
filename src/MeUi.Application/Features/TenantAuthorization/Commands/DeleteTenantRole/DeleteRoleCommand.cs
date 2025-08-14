using MediatR;
using MeUi.Application.Commands;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Commands.DeleteTenantRole;

public record DeleteTenantRoleCommand : BaseTenantCommand, IRequest<Guid>, ITenantRequest
{
    public Guid Id { get; set; }
}