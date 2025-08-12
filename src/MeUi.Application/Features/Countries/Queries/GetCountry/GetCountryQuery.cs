using MediatR;
using MeUi.Application.Models;

namespace MeUi.Application.Features.Countries.Queries.GetCountry;

public record GetCountryQuery : IRequest<CountryDto>
{
    public Guid Id { get; init; }
}
