using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Queries.GetProtocol;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Protocols;

public class GetProtocolByIdEndpoint : BaseEndpoint<GetProtocolQuery, ProtocolDto>, IPermissionProvider
{
    public static string Permission => "READ:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/protocols/{id}");
        Description(x => x.WithTags("Protocol Management")
            .WithSummary("Get protocol by ID")
            .WithDescription("Retrieves a specific protocol by ID. Requires READ:PROTOCOL permission."));
    }

    public override async Task HandleAsync(GetProtocolQuery req, CancellationToken ct)
    {
        ProtocolDto protocol = await Mediator.Send(req, ct);
        await SendSuccessAsync(protocol, "Protocol retrieved successfully", ct);
    }
}
