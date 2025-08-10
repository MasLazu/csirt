using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantUserAccessiblePages;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantUserAccessiblePagesEndpoint : BaseEndpoint<GetTenantUserAccessiblePagesRequest, IEnumerable<PageDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_USER_PAGES";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/users/{UserId}/accessible-pages");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get accessible pages for a tenant user"));
    }

    public override async Task HandleAsync(GetTenantUserAccessiblePagesRequest req, CancellationToken ct)
    {
        var query = new GetTenantUserAccessiblePagesQuery(req.UserId);
        var pages = await Mediator.Send(query, ct);
        await SendSuccessAsync(pages, "Tenant user accessible pages retrieved successfully", ct);
    }
}