using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Countries.Queries.GetCountry;

public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, CountryDto>
{
    private readonly IRepository<Country> _countryRepository;

    public GetCountryQueryHandler(IRepository<Country> countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<CountryDto> Handle(GetCountryQuery request, CancellationToken ct)
    {
        Country? country = await _countryRepository.GetByIdAsync(request.Id, ct);
        if (country == null)
        {
            throw new NotFoundException($"Country with ID {request.Id} not found");
        }

        return country.Adapt<CountryDto>();
    }
}
