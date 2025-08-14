using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.BulkAssignAsnRegistriesToTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAsnRegistries;

public class BulkAssignAsnRegistriesToTenantEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<BulkAssignAsnRegistriesToTenantCommand, BulkAssignAsnRegistriesToTenantEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "CREATE:ASN_REGISTRY";
    public static string Permission => "CREATE:TENANT_ASN";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{tenantId}/asn-registries/bulk");
        Description(x => x.WithTags("Tenant ASN Assignment")
            .WithSummary("Bulk assign ASN registries to tenant")
            .WithDescription("Assigns multiple ASN registries to a specific tenant in a single operation. Requires CREATE:TENANT_ASN or CREATE:ASN_REGISTRY permission."));
    }

    protected override async Task HandleAuthorizedAsync(BulkAssignAsnRegistriesToTenantCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("ASN Registries bulk assigned to tenant successfully", ct);
    }
}
