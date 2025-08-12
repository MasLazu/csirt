using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPages;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Page;

public class GetTenantPagesEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageDto>>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:PAGE";
    public static string Permission => "READ:TENANT_PAGE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/pages");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get pages accessible in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<PageDto> pages = await Mediator.Send(new GetTenantPagesQuery(), ct);
        await SendSuccessAsync(pages, "Tenant pages retrieved successfully", ct);
    }
}