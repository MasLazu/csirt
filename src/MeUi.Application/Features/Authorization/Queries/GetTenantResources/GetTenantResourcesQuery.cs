using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetTenantResources;

public record GetTenantResourcesQuery : IRequest<IEnumerable<ResourceDto>>;