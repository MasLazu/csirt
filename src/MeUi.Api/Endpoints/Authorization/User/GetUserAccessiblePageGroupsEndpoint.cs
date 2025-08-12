using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetUserAccessiblePages;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.User;

public class GetUserAccessiblePageGroupsEndpoint : BaseEndpoint<GetUserAccessiblePagesQuery, IEnumerable<PageGroupDto>>, IPermissionProvider
{
    public static string Permission => "READ:USER_ACCESSIBLE_PAGES";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/accessible-page-groups");
        Description(x => x.WithTags("User Authorization").WithSummary("Get accessible page groups for a specific user"));
    }

    public override async Task HandleAsync(GetUserAccessiblePagesQuery req, CancellationToken ct)
    {
        IEnumerable<PageGroupDto> pageGroups = await Mediator.Send(req, ct);
        await SendSuccessAsync(pageGroups, "User accessible page groups retrieved successfully", ct);
    }
}
