using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Authorization.Queries.GetResources;

public class GetResourcesQuery : IRequest<IEnumerable<ResourceDto>>;