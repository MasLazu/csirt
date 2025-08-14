using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Queries.GetTenantsPaginated;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class GetTenantsPaginatedEndpoint : BaseAuthorizedEndpoint<GetTenantsPaginatedQuery, PaginatedDto<TenantDto>, GetTenantsPaginatedEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:TENANT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants");
        Description(x => x.WithTags("Tenant Management").WithSummary("Get paginated list of tenants"));
    }

    public override async Task HandleAuthorizedAsync(GetTenantsPaginatedQuery req, Guid userId, CancellationToken ct)
    {
        PaginatedDto<TenantDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Tenants retrieved successfully", ct);
    }
}