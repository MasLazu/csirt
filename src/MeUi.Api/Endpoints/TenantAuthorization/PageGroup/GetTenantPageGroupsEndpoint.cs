using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantPageGroups;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.PageGroup;

public class GetTenantPageGroupsEndpoint : BaseTenantAuthorizedEndpoint<GetTenantPageGroupsQuery, IEnumerable<PageGroupDto>, GetTenantPageGroupsEndpoint>, ITenantPermissionProvider, IPermissionProvider
{
    public static string TenantPermission => "READ:PAGE_GROUP";
    public static string Permission => "READ:TENANT_PAGE_GROUP";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/authorization/page-groups");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get page groups accessible in tenant context"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantPageGroupsQuery req, Guid userId, CancellationToken ct)
    {
        IEnumerable<PageGroupDto> pageGroups = await Mediator.Send(req, ct);
        await SendSuccessAsync(pageGroups, "Tenant page groups retrieved successfully", ct);
    }
}