using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetPages;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Page;

public class GetPagesEndpoint : BaseEndpointWithoutRequest<IEnumerable<PageDto>>, IPermissionProvider
{
    public static string Permission => "READ:PAGE";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/pages");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all pages"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<PageDto> result = await Mediator.Send(new GetPagesQuery(), ct);
        await SendSuccessAsync(result, "Pages retrieved successfully", ct);
    }
}