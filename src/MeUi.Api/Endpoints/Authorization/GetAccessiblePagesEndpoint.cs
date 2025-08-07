using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetAccessiblePages;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Api.Endpoints.Authorization;

public class GetAccessiblePagesRequest
{
    public Guid UserId { get; set; }
}

public class GetAccessiblePagesEndpoint : BaseEndpoint<GetAccessiblePagesQuery, IEnumerable<PageGroupDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/users/{userId}/accessible-pages");
        Description(x => x.WithTags("User").WithSummary("Get accessible pages for user"));
    }

    public override async Task HandleAsync(GetAccessiblePagesQuery req, CancellationToken ct)
    {
        IEnumerable<PageGroupDto> result = await Mediator.Send(req, ct);
        await SendSuccessAsync(result, "Accessible pages retrieved successfully", ct);
    }
}