using MeUi.Api.Endpoints;
using MeUi.Application.Features.TenantUsers.Queries.GetTenantUserById;
using MeUi.Application.Models;
using MeUi.Api.Endpoints.TenantAuthorization.User;
using MeUi.Api.Models;

namespace MeUi.Api.Endpoints.TenantAuthorization.Me;

public class GetTenantUserMeEndpoint : BaseTenantAuthorizedEndpoint<GetTenantUserMeRequest, TenantUserDto, GetTenantUserByIdEndpoint>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/tenants/{tenantId}/users/me");
        Description(x => x.WithTags("Tenant User Management").WithSummary("Get current tenant user profile"));
    }

    protected override async Task HandleAuthorizedAsync(GetTenantUserMeRequest req, Guid userId, CancellationToken ct)
    {
        var query = new GetTenantUserByIdQuery
        {
            Id = userId,
            TenantId = req.TenantId
        };

        TenantUserDto user = await Mediator.Send(query, ct);
        await SendSuccessAsync(user, "Tenant user profile retrieved successfully", ct);
    }
}
