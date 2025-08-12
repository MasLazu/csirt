using System.IdentityModel.Tokens.Jwt;
using FastEndpoints.Security;
using MeUi.Api.Endpoints;
using MeUi.Application.Exceptions;
using MeUi.Application.Features.Authorization.Queries.GetAccessiblePages;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Me;

public class GetAccessiblePageGroupsEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageGroupDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/me/accessible-page-groups");
        Description(x => x.WithTags("User Authorization").WithSummary("Get accessible page groups for current user"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? sub = User.ClaimValue("sub");

        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var req = new GetAccessiblePagesQuery
        {
            UserId = Guid.Parse(sub),
        };

        IEnumerable<PageGroupDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Accessible page groups retrieved successfully", ct);
    }
}