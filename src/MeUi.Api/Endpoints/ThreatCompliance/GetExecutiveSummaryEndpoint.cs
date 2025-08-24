using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetExecutiveSummary;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetExecutiveSummaryEndpoint : BaseAuthorizedEndpoint<GetExecutiveSummaryQuery, List<ExecutiveSummaryDto>, GetExecutiveSummaryEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/executive-summary");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get executive summary")
            .WithDescription("Retrieves executive summary metrics for threat compliance. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetExecutiveSummaryQuery req, Guid userId, CancellationToken ct)
    {
        List<ExecutiveSummaryDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} executive summary metrics", ct);
    }
}
