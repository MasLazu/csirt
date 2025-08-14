using MeUi.Api.Endpoints;
using MeUi.Application.Features.Tenants.Queries.GetTenantById;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Tenants;

public class GetTenantByIdEndpoint : BaseAuthorizedEndpoint<GetTenantByIdQuery, TenantDto, GetTenantByIdEndpoint>, IPermissionProvider
{
    public static string Permission => "READ:TENANT";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{id}");
        Description(x => x.WithTags("Tenant Management").WithSummary("Get tenant by ID"));
    }

    public override async Task HandleAuthorizedAsync(GetTenantByIdQuery req, Guid userId, CancellationToken ct)
    {
        TenantDto tenant = await Mediator.Send(req, ct);
        await SendSuccessAsync(tenant, "Tenant retrieved successfully", ct);
    }
}
