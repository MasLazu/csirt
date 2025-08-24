using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetExecutiveSummary;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetExecutiveSummaryEndpoint : BaseAuthorizedEndpoint<GetExecutiveSummaryQuery, List<ExecutiveSummaryMetricDto>, GetExecutiveSummaryEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/executive-summary");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get executive summary metrics")
            .WithDescription("Retrieves high-level executive summary metrics for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetExecutiveSummaryQuery req, Guid userId, CancellationToken ct)
    {
        List<ExecutiveSummaryMetricDto> summary = await Mediator.Send(req, ct);
        await SendSuccessAsync(summary, $"Retrieved {summary.Count} executive summary metrics", ct);
    }
}
