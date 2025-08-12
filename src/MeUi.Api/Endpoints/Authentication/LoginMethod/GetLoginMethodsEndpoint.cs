using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authentication.Queries.GetLoginMethods;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authentication;

public class GetLoginMethodsEndpoint : BaseEndpointWithoutRequest<IEnumerable<LoginMethodDto>>, IPermissionProvider
{
    public static string Permission => "READ:LOGIN_METHOD";

    public override void ConfigureEndpoint()
    {
        Get("api/v1/auth/login-methods");
        Description(x => x.WithTags("Authentication").WithSummary("Get login methods"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<LoginMethodDto> activeLoginMethods = await Mediator.Send(new GetLoginMethodsQuery(), ct);
        await SendSuccessAsync(activeLoginMethods, "Actions retrieved successfully", ct);
    }
}