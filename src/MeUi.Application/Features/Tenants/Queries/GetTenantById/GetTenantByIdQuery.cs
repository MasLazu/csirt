using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Tenants.Queries.GetTenantById;

public class GetTenantByIdQuery : IRequest<TenantDto>
{
    public Guid Id { get; init; }
}
