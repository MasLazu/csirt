using MeUi.Api.Endpoints;
using MeUi.Application.Features.ThreatCompliance.Queries.GetAttackCategoryAnalysis;
using MeUi.Application.Models.ThreatCompliance;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.ThreatCompliance;

public class GetAttackCategoryAnalysisEndpoint : BaseAuthorizedEndpoint<GetAttackCategoryAnalysisQuery, List<AttackCategoryDto>, GetAttackCategoryAnalysisEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_COMPLIANCE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-compliance/attack-categories");
        Description(x => x.WithTags("Threat Compliance")
            .WithSummary("Get attack categories")
            .WithDescription("Retrieves attack category analysis for threat compliance. Requires READ:THREAT_COMPLIANCE permission."));
    }

    public override async Task HandleAuthorizedAsync(GetAttackCategoryAnalysisQuery req, Guid userId, CancellationToken ct)
    {
        List<AttackCategoryDto> resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} attack categories", ct);
    }
}
