using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetKpiTrend;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetKpiTrendEndpoint : BaseAuthorizedEndpoint<GetKpiTrendQuery, List<KpiTrendPointDto>, GetKpiTrendEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/kpi-trend");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get KPI trend")
            .WithDescription("Retrieves KPI trend points for threat compliance. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetKpiTrendQuery req, Guid userId, CancellationToken ct)
    {
        List<KpiTrendPointDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} KPI points", ct);
    }
}
