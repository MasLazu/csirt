using MediatR;

namespace MeUi.Application.Features.Countries.Commands.CreateCountry;

public record CreateCountryCommand : IRequest<Guid>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
