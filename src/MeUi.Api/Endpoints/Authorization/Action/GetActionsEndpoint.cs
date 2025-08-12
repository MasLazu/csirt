using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authorization.Queries.GetActions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authorization.Action;

public class GetActionsEndpoint : BaseEndpointWithoutRequest<IEnumerable<ActionDto>>, IPermissionProvider
{
    public static string Permission => "READ:ACTION";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/authorization/actions");
        Description(x => x.WithTags("System Authorization").WithSummary("Get all actions"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<ActionDto> actions = await Mediator.Send(new GetActionsQuery(), ct);
        await SendSuccessAsync(actions, "Actions retrieved successfully", ct);
    }
}