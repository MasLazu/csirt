using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Models;
using MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class GetTenantsPaginatedEndpoint : BaseEndpoint<GetTenantsPaginatedQuery, PaginatedResult<TenantDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants");
        Description(x => x.WithTags("Tenants").WithSummary("Get paginated list of tenants"));
        // TODO: Add authorization for super admin only
    }

    public override async Task HandleAsync(GetTenantsPaginatedQuery req, CancellationToken ct)
    {
        var result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Tenants retrieved successfully", ct);
    }
}