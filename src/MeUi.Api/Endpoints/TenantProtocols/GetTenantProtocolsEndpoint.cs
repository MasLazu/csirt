using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Queries.GetProtocolsPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantProtocols;

public class GetTenantProtocolsEndpoint : BaseTenantAuthorizedEndpoint<GetProtocolsPaginatedQuery, PaginatedDto<ProtocolDto>, GetTenantProtocolsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:PROTOCOL";
    public static string Permission => "READ:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/protocols");
        Description(x => x.WithTags("Tenant Protocols")
            .WithSummary("Get protocols (tenant scope)")
            .WithDescription("Retrieves a paginated list of protocols for tenant UI consumption. Requires READ:PROTOCOL permission."));
    }

    protected override async Task HandleAuthorizedAsync(GetProtocolsPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<ProtocolDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Items.Count()} protocols", ct);
    }
}
