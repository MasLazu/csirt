using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.RemoveAsnFromTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAsnRegistries;

public class RemoveAsnRegistryFromTenantEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<RemoveAsnFromTenantCommand, RemoveAsnRegistryFromTenantEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "DELETE:ASN_REGISTRY";
    public static string Permission => "DELETE:TENANT_ASN";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/tenants/{tenantId}/asn-registries/{asnId}");
        Description(x => x.WithTags("Tenant ASN Assignment")
            .WithSummary("Remove ASN registry from tenant")
            .WithDescription("Removes an ASN registry assignment from a specific tenant. Requires DELETE:TENANT_ASN or DELETE:ASN_REGISTRY permission."));
    }

    protected override async Task HandleAuthorizedAsync(RemoveAsnFromTenantCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("ASN Registry removed from tenant successfully", ct);
    }
}
