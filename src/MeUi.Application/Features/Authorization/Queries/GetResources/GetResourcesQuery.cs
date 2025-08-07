using MediatR;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetResources;

public record GetResourcesQuery : IRequest<IEnumerable<ResourceDto>>;