using MeUi.Api.Endpoints;
using MeUi.Application.Features.Protocols.Commands.CreateProtocol;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Protocols;

public class CreateProtocolEndpoint : BaseAuthorizedEndpoint<CreateProtocolCommand, Guid, CreateProtocolEndpoint>, IPermissionProvider
{
    public static string Permission => "CREATE:PROTOCOL";

    public override void ConfigureEndpoint()
    {
        Post("api/v1/protocols");
        Description(x => x.WithTags("Protocol Management")
            .WithSummary("Create a new protocol")
            .WithDescription("Creates a new protocol. Requires CREATE:PROTOCOL permission."));
    }
    public override async Task HandleAuthorizedAsync(CreateProtocolCommand req, Guid userId, CancellationToken ct)
    {
        Guid id = await Mediator.Send(req, ct);
        await SendSuccessAsync(id, "Protocol created successfully", ct);
    }
}
