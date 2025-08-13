using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Countries.Commands.DeleteCountry;

public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Guid>
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCountryCommandHandler(
        IRepository<Country> countryRepository,
        IRepository<ThreatEvent> threatEventRepository,
        IUnitOfWork unitOfWork)
    {
        _countryRepository = countryRepository;
        _threatEventRepository = threatEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteCountryCommand request, CancellationToken ct)
    {
        Country country = await _countryRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"Country with ID {request.Id} not found");

        bool isReferenced = await _threatEventRepository.ExistsAsync(
            te => te.SourceCountryId == request.Id || te.DestinationCountryId == request.Id, ct);

        if (isReferenced)
        {
            throw new ConflictException($"Cannot delete country '{country.Name}' because it is referenced by existing threat events");
        }

        await _countryRepository.DeleteAsync(country, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return country.Id;
    }
}
