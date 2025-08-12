using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantActions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Action;

public class GetTenantActionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<ActionDto>>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ACTION";
    public static string Permission => "READ:TENANT_ACTION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/actions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get actions accessible in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<ActionDto> actions = await Mediator.Send(new GetTenantActionsQuery(), ct);
        await SendSuccessAsync(actions, "Tenant actions retrieved successfully", ct);
    }
}