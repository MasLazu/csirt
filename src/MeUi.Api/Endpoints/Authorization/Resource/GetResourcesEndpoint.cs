using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetResources;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Resource;

public class GetResourcesEndpoint : BaseAuthorizedEndpointWithoutRequest<IEnumerable<ResourceDto>, GetResourcesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:RESOURCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/resources");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all resources"));
    }
    public override async Task HandleAuthorizedAsync(Guid userId, CancellationToken ct)
    {
        IEnumerable<ResourceDto> resources = await Mediator.Send(new GetResourcesQuery(), ct);
        await SendSuccessAsync(resources, "Resources retrieved successfully", ct);
    }
}