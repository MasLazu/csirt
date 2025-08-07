using MeUi.Application.Features.Authorization.Queries.GetPages;
using MeUi.Application.Features.Authorization.Models;

namespace MeUi.Api.Endpoints.Authorization;

public class GetPagesEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/pages");
        Description(x => x.WithTags("Page").WithSummary("Get all pages"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<PageDto> result = await Mediator.Send(new GetPagesQuery(), ct);
        await SendSuccessAsync(result, "Pages retrieved successfully", ct);
    }
}