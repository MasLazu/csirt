using MediatR;

namespace MeUi.Application.Features.TenantUsers.Commands.DeleteTenantUser;

public record DeleteTenantUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}