using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatEvents.Commands.UpdateThreatEvent;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatEvents;

public class UpdateThreatEventEndpoint : BaseEndpointWithoutResponse<UpdateThreatEventCommand>, IPermissionProvider
{
    public static string Permission => "UPDATE:THREAT_EVENT";

    public override void ConfigureEndpoint()
    {
        Put("api/v1/threat-events/{id}");
        Description(x => x.WithTags("Threat Event Management")
            .WithSummary("Update a threat event")
            .WithDescription("Updates an existing threat event. Note: Updating historical time-series data should be done carefully. Requires UPDATE:THREAT_EVENT permission."));
    }

    public override async Task HandleAsync(UpdateThreatEventCommand req, CancellationToken ct)
    {
        await Mediator.Send(req, ct);
        await SendSuccessAsync("Threat event updated successfully", ct);
    }
}
