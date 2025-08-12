using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Countries.Commands.UpdateCountry;

public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Guid>
{
    private readonly IRepository<Country> _countryRepository;

    public UpdateCountryCommandHandler(IRepository<Country> countryRepository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<Guid> Handle(UpdateCountryCommand request, CancellationToken ct)
    {
        Country country = await _countryRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Country with ID {request.Id} not found");

        if (await _countryRepository.ExistsAsync(c => c.Code == request.Code && c.Id != request.Id, ct))
        {
            throw new ConflictException($"Country with code '{request.Code}' already exists");
        }

        if (await _countryRepository.ExistsAsync(c => c.Name == request.Name && c.Id != request.Id, ct))
        {
            throw new ConflictException($"Country with name '{request.Name}' already exists");
        }

        country.Code = request.Code;
        country.Name = request.Name;
        country.UpdatedAt = DateTime.UtcNow;

        await _countryRepository.UpdateAsync(country, ct);
        return country.Id;
    }
}
