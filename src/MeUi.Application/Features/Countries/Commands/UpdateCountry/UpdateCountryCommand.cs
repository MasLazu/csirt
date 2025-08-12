using MediatR;

namespace MeUi.Application.Features.Countries.Commands.UpdateCountry;

public record UpdateCountryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
