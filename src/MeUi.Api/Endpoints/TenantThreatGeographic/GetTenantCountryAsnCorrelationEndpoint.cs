using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantThreatGeographic.Queries.GetCountryAsnCorrelation;
using MeUi.Application.Models.ThreatGeographic;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.TenantThreatGeographic;

public class GetTenantCountryAsnCorrelationEndpoint : BaseTenantAuthorizedEndpoint<GetTenantCountryAsnCorrelationQuery, List<CountryAsnCorrelationDto>, GetTenantCountryAsnCorrelationEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string Permission => "READ:THREAT_GEOGRAPHIC";
    public static string TenantPermission => "READ:TENANT_THREAT_GEOGRAPHIC";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/threat-geographic/country-asn-correlation");
        Description(x => x.WithTags("Tenant Threat Geographic")
            .WithSummary("Get tenant country-ASN correlation matrix")
            .WithDescription("Retrieves correlation analysis between countries and ASN networks for tenant-specific threat intelligence. Requires READ:TENANT_THREAT_GEOGRAPHIC permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantCountryAsnCorrelationQuery req, Guid userId, CancellationToken ct)
    {
        var correlations = await Mediator.Send(req, ct);
        await SendSuccessAsync(correlations, $"Retrieved {correlations.Count} country-ASN correlations for tenant {req.TenantId}", ct);
    }
}