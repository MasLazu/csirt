using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantPageGroups;

public record GetTenantPageGroupsQuery : IRequest<IEnumerable<PageGroupDto>>;