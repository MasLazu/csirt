using MeUi.Application.Features.Authorization.Models;
using MeUi.Application.Features.Authorization.Queries.GetTenantPages;
using MeUi.Application.Interfaces;

namespace MeUi.Api.Endpoints.Authorization;

public class GetTenantPagesEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageDto>>, ITenantPermissionProvider
{
    public static string Permission => "READ:TENANT_PAGE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenant/pages");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get tenant pages"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var pages = await Mediator.Send(new GetTenantPagesQuery(), ct);
        await SendSuccessAsync(pages, "Tenant pages retrieved successfully", ct);
    }
}