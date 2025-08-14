using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantActions;

public record GetTenantActionsQuery : IRequest<IEnumerable<ActionDto>>, ITenantRequest
{
    public Guid TenantId { get; set; }
}