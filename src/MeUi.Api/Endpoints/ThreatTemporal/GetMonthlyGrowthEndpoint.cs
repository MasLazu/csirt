using MeUi.Api.Endpoints;
using MeUi.Application.Interfaces;
using MeUi.Application.Features.ThreatTemporal.Queries.GetMonthlyGrowth;
using MeUi.Application.Models.ThreatTemporal;

namespace MeUi.Api.Endpoints.ThreatTemporal;

public class GetMonthlyGrowthEndpoint : BaseAuthorizedEndpoint<GetMonthlyGrowthQuery, List<MonthlyGrowthDto>, GetMonthlyGrowthEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:THREAT_TEMPORAL";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/threat-temporal/monthly-growth");
        Description(x => x.WithTags("Threat Temporal").WithSummary("Monthly growth rate").WithDescription("Returns monthly growth by category."));
    }

    public override async Task HandleAuthorizedAsync(GetMonthlyGrowthQuery req, Guid userId, CancellationToken ct)
    {
        var resp = await Mediator.Send(req, ct);
        await SendSuccessAsync(resp, $"Retrieved {resp?.Count ?? 0} rows", ct);
    }
}
