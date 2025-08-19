using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatIntelligentOverview.Queries.GetThreatActivityTimeline;
using MeUi.Application.Models.ThreatIntelligentOverview;
using MeUi.Application.Interfaces;
using System.Collections.Generic;

namespace MeUi.Api.Endpoints.ThreatIntelligentOverview;

public class GetThreatActivityTimelineEndpoint : BaseAuthorizedEndpoint<GetThreatActivityTimelineQuery, List<TimelineDataPointDto>, GetThreatActivityTimelineEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_INTELLIGENT_OVERVIEW";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-intelligent-overview/activity-timeline");
        Description(x => x.WithTags("Threat Intelligent Overview")
            .WithSummary("Get threat activity timeline")
            .WithDescription("Retrieves threat activity timeline data points for threat intelligence overview. Requires READ:THREAT_INTELLIGENT_OVERVIEW permission."));
    }

    public override async Task HandleAuthorizedAsync(GetThreatActivityTimelineQuery req, Guid userId, CancellationToken ct)
    {
        var timeline = await Mediator.Send(req, ct);
        await SendSuccessAsync(timeline, $"Retrieved {timeline.Count} timeline data points", ct);
    }
}
