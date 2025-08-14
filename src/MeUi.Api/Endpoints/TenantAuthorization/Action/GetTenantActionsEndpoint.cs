using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantActions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Action;

public class GetTenantActionsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantActionsQuery, IEnumerable<ActionDto>, GetTenantActionsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ACTION";
    public static string Permission => "READ:TENANT_ACTION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/actions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get actions accessible in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantActionsQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<ActionDto> actions = await Mediator.Send(req, ct);
        await SendSuccessAsync(actions, "Tenant actions retrieved successfully", ct);
    }
}