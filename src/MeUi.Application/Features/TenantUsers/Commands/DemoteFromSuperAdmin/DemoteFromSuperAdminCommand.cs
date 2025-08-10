using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.DemoteFromSuperAdmin;

public record DemoteFromSuperAdminCommand : IRequest<Unit>
{
    public Guid TenantUserId { get; init; }
}