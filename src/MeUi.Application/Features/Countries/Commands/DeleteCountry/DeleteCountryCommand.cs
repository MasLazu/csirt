using MediatR;

namespace MeUi.Application.Features.Countries.Commands.DeleteCountry;

public record DeleteCountryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
}
