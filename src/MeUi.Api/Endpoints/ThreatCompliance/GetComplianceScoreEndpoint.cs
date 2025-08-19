using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetComplianceScore;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetComplianceScoreEndpoint : BaseAuthorizedEndpoint<GetComplianceScoreQuery, ComplianceScoreDto, GetComplianceScoreEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/compliance-score");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get compliance score")
            .WithDescription("Retrieves overall compliance score for a date range. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetComplianceScoreQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, "Retrieved compliance score", ct);
    }
}
