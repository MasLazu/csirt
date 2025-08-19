using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatCategoryAnalysis;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetThreatCategoryAnalysisEndpoint : BaseAuthorizedEndpoint<GetThreatCategoryAnalysisQuery, List<ThreatCategoryAnalysisDto>, GetThreatCategoryAnalysisEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/threat-category-analysis");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get threat category analysis")
            .WithDescription("Retrieves threat category analysis for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatCategoryAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        var analysis = await Mediator.Send(req, ct);
        await SendSuccessAsync(analysis, $"Retrieved {analysis.Count} threat category analysis records", ct);
    }
}
