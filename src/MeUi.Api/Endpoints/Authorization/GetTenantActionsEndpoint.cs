using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantActions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantActionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<ActionDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_ACTION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/actions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get tenant actions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var actions = await Mediator.Send(new GetTenantActionsQuery(), ct);
        await SendSuccessAsync(actions, "Tenant actions retrieved successfully", ct);
    }
}