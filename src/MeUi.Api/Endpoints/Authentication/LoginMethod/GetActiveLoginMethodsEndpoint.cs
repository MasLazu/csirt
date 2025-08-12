using MeUi.Api.Endpoints;
using MeUi.Application.Features.Authentication.Queries.GetActiveLoginMethods;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;

namespace MeUi.Api.Endpoints.Authentication.LoginMethod;

public class GetActiveLoginMethodsEndpoint : BaseEndpointWithoutRequest<IEnumerable<LoginMethodDto>>
{
    public override void ConfigureEndpoint()
    {
        Get("api/v1/auth/login-methods/active");
        Description(x => x.WithTags("Authentication").WithSummary("Get all currently active login methods available for authentication"));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<LoginMethodDto> activeLoginMethods = await Mediator.Send(new GetActiveLoginMethodsQuery(), ct);
        await SendSuccessAsync(activeLoginMethods, "Login methods retrieved successfully", ct);
    }
}