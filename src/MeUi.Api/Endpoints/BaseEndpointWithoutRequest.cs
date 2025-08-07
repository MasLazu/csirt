using FastEndpoints;
using MediatR;
using MeUi.Api.Models;

namespace MeUi.Api.Endpoints;

public abstract class BaseEndpointWithoutRequest<TResponse> : EndpointWithoutRequest<ApiResponse<TResponse>>
{
    protected IMediator Mediator => Resolve<IMediator>();

    public override void Configure()
    {
        DontCatchExceptions();
        ConfigureEndpoint();
    }

    public abstract void ConfigureEndpoint();

    protected async Task SendSuccessAsync(TResponse data, string message, CancellationToken ct)
    {
        await SendOkAsync(new SuccessApiResponse<TResponse>(data, message), ct);
    }
}