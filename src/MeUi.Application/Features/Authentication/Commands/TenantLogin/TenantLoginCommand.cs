using MediatR;
using MeUi.Application.Features.Authentication.Models;

namespace MeUi.Application.Features.Authentication.Commands.TenantLogin;

public record TenantLoginCommand : IRequest<TenantTokenResponse>
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public Guid? TenantId { get; init; }
}