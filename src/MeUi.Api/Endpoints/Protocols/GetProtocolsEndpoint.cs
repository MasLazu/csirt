using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Queries.GetProtocolsPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Protocols;

public class GetProtocolsEndpoint : BaseAuthorizedEndpoint<GetProtocolsPaginatedQuery, PaginatedDto<ProtocolDto>, GetProtocolsEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/protocols");
        Description(x => x.WithTags("Protocol Management")
            .WithSummary("Get paginated list of protocols")
            .WithDescription("Retrieves a paginated list of protocols with optional search & sorting. Requires READ:PROTOCOL permission."));
    }
    public override async Task HandleAuthorizedAsync(GetProtocolsPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<ProtocolDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, $"Retrieved {result.Items.Count()} protocols", ct);
    }
}
