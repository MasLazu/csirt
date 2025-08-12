using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.AsnRegistries.Commands.UpdateAsnRegistry;

public class UpdateAsnRegistryCommandHandler : IRequestHandler<UpdateAsnRegistryCommand, Guid>
{
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAsnRegistryCommandHandler(
        IRepository<AsnRegistry> asnRegistryRepository,
        IUnitOfWork unitOfWork)
    {
        _asnRegistryRepository = asnRegistryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateAsnRegistryCommand request, CancellationToken ct)
    {
        AsnRegistry asnRegistry = await _asnRegistryRepository.FirstOrDefaultAsync(
            asn => asn.Id == request.Id, ct) ??
            throw new NotFoundException($"ASN Registry with ID {request.Id} not found");

        // Check if the new ASN number conflicts with existing ones (excluding current record)
        bool asnExists = await _asnRegistryRepository.ExistsAsync(
            asn => asn.Number == request.Number && asn.Id != request.Id, ct);

        if (asnExists)
        {
            throw new ConflictException($"ASN Registry with number '{request.Number}' already exists");
        }

        asnRegistry.Number = request.Number;
        asnRegistry.Description = request.Description;

        await _asnRegistryRepository.UpdateAsync(asnRegistry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return asnRegistry.Id;
    }
}
