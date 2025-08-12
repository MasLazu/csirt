using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.TenantAuthorization.Queries.GetTenantResources;

public record GetTenantResourcesQuery : IRequest<IEnumerable<ResourceDto>>;