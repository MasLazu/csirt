using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetPageGroups;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.PageGroup;

public class GetPageGroupsEndpoint : BaseAuthorizedEndpointWithoutRequest<IEnumerable<PageGroupDto>, GetPageGroupsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:PAGE_GROUP";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/page-groups");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all page groups"));
    }

    public override async Task HandleAuthorizedAsync(Guid userId, CancellationToken ct)
    {
        IEnumerable<PageGroupDto> result = await Mediator.Send(new GetPageGroupsQuery(), ct);
        await SendSuccessAsync(result, "Page groups retrieved successfully", ct);
    }
}