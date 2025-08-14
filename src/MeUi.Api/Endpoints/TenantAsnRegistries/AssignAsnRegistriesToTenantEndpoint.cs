using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Commands.AssignAsnRegistriesToTenant;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.TenantAsnRegistries;

public class AssignAsnRegistriesToTenantEndpoint : BaseTenantAuthorizedEndpointWithoutResponse<AssignAsnRegistryToTenantCommand, AssignAsnRegistriesToTenantEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "CREATE:ASN_REGISTRY";
    public static string Permission => "CREATE:TENANT_ASN";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/tenants/{tenantId}/asn-registries");
        Description(x => x.WithTags("Tenant ASN Assignment")
            .WithSummary("Assign ASN registry to tenant")
            .WithDescription("Assigns an ASN registry to a specific tenant. Requires CREATE:TENANT_ASN or CREATE:ASN_REGISTRY permission."));
    }

    protected override async Task HandleAuthorizedAsync(AssignAsnRegistryToTenantCommand req, Guid userId, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("ASN Registry assigned to tenant successfully", ct);
    }
}
