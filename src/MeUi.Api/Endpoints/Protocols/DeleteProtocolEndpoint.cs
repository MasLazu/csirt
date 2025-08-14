using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Commands.DeleteProtocol;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Protocols;

public class DeleteProtocolEndpoint : BaseAuthorizedEndpoint<DeleteProtocolCommand, Guid, DeleteProtocolEndpoint>, IPermissionProvider
{
    public static string Permission => "DELETE:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/protocols/{id}");
        Description(x => x.WithTags("Protocol Management")
            .WithSummary("Delete a protocol")
            .WithDescription("Deletes a protocol if not referenced by threat events. Requires DELETE:PROTOCOL permission."));
    }
    public override async Task HandleAuthorizedAsync(DeleteProtocolCommand req, Guid userId, CancellationToken ct)
    {
        Guid id = await Mediator.Send(req, ct);
        await SendSuccessAsync(id, "Protocol deleted successfully", ct);
    }
}
