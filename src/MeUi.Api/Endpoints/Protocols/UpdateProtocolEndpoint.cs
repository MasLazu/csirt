using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Commands.UpdateProtocol;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Protocols;

public class UpdateProtocolEndpoint : BaseEndpoint<UpdateProtocolCommand, Guid>, IPermissionProvider
{
    public static string Permission => "UPDATE:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/protocols/{id}");
        Description(x => x.WithTags("Protocol Management")
            .WithSummary("Update an existing protocol")
            .WithDescription("Updates an existing protocol's name. Requires UPDATE:PROTOCOL permission."));
    }

    public override async Task HandleAsync(UpdateProtocolCommand req, CancellationToken ct)
    {
        Guid id = await Mediator.Send(req, ct);
        await SendSuccessAsync(id, "Protocol updated successfully", ct);
    }
}
