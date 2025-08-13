using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Commands.DeleteThreatEvent;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class DeleteThreatEventEndpoint : BaseEndpointWithoutResponse<DeleteThreatEventCommand>, IPermissionProvider
{
    public static string Permission => "DELETE:THREAT_EVENT";

    public override void ConfigureEndpoint()
    {
        Delete("api/v1/threat-events/{id}");
        Description(x => x.WithTags("Threat Event Management")
            .WithSummary("Delete a threat event")
            .WithDescription("Deletes a threat event from the time-series database. This operation is permanent. Requires DELETE:THREAT_EVENT permission."));
    }

    public override async Task HandleAsync(DeleteThreatEventCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Threat event deleted successfully", ct);
    }
}
