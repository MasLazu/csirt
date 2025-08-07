using FastEndpoints;
using MediatR;
using MeUi.Api.Models;

namespace MeUi.Api.Endpoints;

public abstract class BaseEndpointWithourRequestResponse : EndpointWithoutRequest<ApiResponse<object>>
{
    protected IMediator Mediator => Resolve<IMediator>();

    public override void Configure()
    {
        DontCatchExceptions();
        ConfigureEndpoint();
    }

    public abstract void ConfigureEndpoint();

    protected async Task SendSuccessAsync(string message, CancellationToken ct)
    {
        await SendOkAsync(new SuccessApiResponse<object>(null, message), ct);
    }
}