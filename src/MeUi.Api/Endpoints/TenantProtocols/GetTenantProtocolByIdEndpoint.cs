using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Queries.GetProtocol;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantProtocols;

public class GetTenantProtocolByIdEndpoint : BaseEndpoint<GetProtocolQuery, ProtocolDto>, IPermissionProvider
{
    public static string Permission => "READ:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/{tenantId}/protocols/{id}");
        Description(x => x.WithTags("Tenant Protocols")
            .WithSummary("Get protocol by ID (tenant scope)")
            .WithDescription("Retrieves a protocol by ID. Protocols are global; tenant scope adds auth context. Requires READ:PROTOCOL permission."));
    }

    public override async Task HandleAsync(GetProtocolQuery req, CancellationToken ct)
    {
        ProtocolDto protocol = await Mediator.Send(req, ct);
        await SendSuccessAsync(protocol, "Protocol retrieved successfully", ct);
    }
}
