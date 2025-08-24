using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetTopThreatCategories;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetTopThreatCategoriesEndpoint : BaseAuthorizedEndpoint<GetTopThreatCategoriesQuery, List<TopCategoryDto>, GetTopThreatCategoriesEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/top-threat-categories");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get top threat categories")
            .WithDescription("Retrieves top threat categories for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetTopThreatCategoriesQuery req, Guid userId, CancellationToken ct)
    {
        List<TopCategoryDto> categories = await Mediator.Send(req, ct);
        await SendSuccessAsync(categories, $"Retrieved {categories.Count} top threat categories", ct);
    }
}
