using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetCurrentRiskLevel;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetCurrentRiskLevelEndpoint : BaseAuthorizedEndpoint<GetCurrentRiskLevelQuery, RiskLevelDto, GetCurrentRiskLevelEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/current-risk");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get current risk level")
            .WithDescription("Retrieves the current risk level for the requested window. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetCurrentRiskLevelQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, "Retrieved current risk level", ct);
    }
}
