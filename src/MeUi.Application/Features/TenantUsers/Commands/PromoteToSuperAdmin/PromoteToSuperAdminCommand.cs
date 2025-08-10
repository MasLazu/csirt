using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.PromoteToSuperAdmin;

public record PromoteToSuperAdminCommand : IRequest<Unit>
{
    public Guid TenantUserId { get; init; }
}