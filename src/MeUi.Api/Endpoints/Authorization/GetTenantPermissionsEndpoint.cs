using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantPermissions;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantPermissionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PermissionDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_PERMISSION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/permissions");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get tenant permissions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var permissions = await Mediator.Send(new GetTenantPermissionsQuery(), ct);
        await SendSuccessAsync(permissions, "Tenant permissions retrieved successfully", ct);
    }
}