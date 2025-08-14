using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPageGroups;

public record GetTenantPageGroupsQuery : IRequest<IEnumerable<PageGroupDto>>, ITenantRequest
{
	public Guid TenantId { get; set; }
}