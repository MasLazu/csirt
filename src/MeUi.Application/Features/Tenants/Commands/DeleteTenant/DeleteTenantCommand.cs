using MediatR;

namespace MeUi.Application.Features.Tenants.Commands.DeleteTenant;

public record DeleteTenantCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}