using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetWeeklyAttackDistribution;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetWeeklyAttackDistributionEndpoint : BaseAuthorizedEndpoint<GetWeeklyAttackDistributionQuery, List<DayOfWeekDto>, GetWeeklyAttackDistributionEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/weekly-distribution");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Weekly attack distribution").WithDescription("Returns events per weekday."));
    }

    public override async Task HandleAuthorizedAsync(GetWeeklyAttackDistributionQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} days", ct);
    }
}
