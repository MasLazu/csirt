using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Queries.GetTenantAsnRegistriesPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAsnRegistries;

public class GetTenantAsnRegistriesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantAsnRegistriesPaginatedQuery, PaginatedDto<AsnRegistryDto>, GetTenantAsnRegistriesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:ASN_REGISTRY";
    public static string Permission => "READ:TENANT_ASN";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/asn-registries");
        Description(x => x.WithTags("Tenant ASN Management")
            .WithSummary("Get tenant's assigned ASN registries")
            .WithDescription("Retrieves a paginated list of ASN registries assigned to the specified tenant. Global admins can access any tenant's ASN registries with READ:TENANT_ASN permission. Tenant users can only access their own tenant's ASN registries with READ:ASN_REGISTRY permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantAsnRegistriesPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<AsnRegistryDto> asnRegistries = await Mediator.Send(req, ct);
        await SendSuccessAsync(asnRegistries, "Tenant ASN Registries retrieved successfully", ct);
    }
}
