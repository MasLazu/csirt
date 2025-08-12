using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.TenantAuthorization.Queries.GetTenantUserAccessiblePages;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Me;

public class GetTenantUserAccessiblePagesEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageGroupDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users/me/accessible-page-groups");
        Description(x => x.WithTags("Tenant Authorization").WithSummary("Get accessible page groups for current user in tenant context"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");

        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var req = new GetTenantUserAccessiblePagesQuery
        {
            UserId = Guid.Parse(sub),
        };

        IEnumerable<PageGroupDto> pages = await Mediator.Send(req, ct);
        await SendSuccessAsync(pages, "Tenant user accessible page groups retrieved successfully", ct);
    }
}