using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;

public record DeleteTenantUserCommand : IRequest<Unit>
    , ITenantRequest
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
}