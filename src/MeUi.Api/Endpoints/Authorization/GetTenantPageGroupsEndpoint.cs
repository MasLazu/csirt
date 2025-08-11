using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantPageGroups;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantPageGroupsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageGroupDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_PAGE_GROUP";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/page-groups");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get tenant page groups"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var pageGroups = await Mediator.Send(new GetTenantPageGroupsQuery(), ct);
        await SendSuccessAsync(pageGroups, "Tenant page groups retrieved successfully", ct);
    }
}