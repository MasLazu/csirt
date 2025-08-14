using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPages;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Page;

public class GetTenantPagesEndpoint : BaseTenantAuthorizedEndpoint<GetTenantPagesQuery, IEnumerable<PageDto>, GetTenantPagesEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:PAGE";
    public static string Permission => "READ:TENANT_PAGE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/pages");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get pages accessible in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantPagesQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<PageDto> pages = await Mediator.Send(req, ct);
        await SendSuccessAsync(pages, "Tenant pages retrieved successfully", ct);
    }
}