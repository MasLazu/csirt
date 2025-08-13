using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Countries.Commands.CreateCountry;

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Guid>
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCountryCommandHandler(IRepository<Country> countryRepository, IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCountryCommand request, CancellationToken ct)
    {
        if (await _countryRepository.ExistsAsync(c => c.Code == request.Code, ct))
        {
            throw new ConflictException($"Country with code '{request.Code}' already exists");
        }

        if (await _countryRepository.ExistsAsync(c => c.Name == request.Name, ct))
        {
            throw new ConflictException($"Country with name '{request.Name}' already exists");
        }

        Country country = request.Adapt<Country>();
        country.Id = Guid.NewGuid();
        country.CreatedAt = DateTime.UtcNow;

        await _countryRepository.AddAsync(country, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return country.Id;
    }
}
