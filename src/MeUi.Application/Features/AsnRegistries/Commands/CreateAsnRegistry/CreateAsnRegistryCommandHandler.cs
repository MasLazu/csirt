using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.AsnRegistries.Commands.CreateAsnRegistry;

public class CreateAsnRegistryCommandHandler : IRequestHandler<CreateAsnRegistryCommand, Guid>
{
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAsnRegistryCommandHandler(
        IRepository<AsnRegistry> asnRegistryRepository,
        IUnitOfWork unitOfWork)
    {
        _asnRegistryRepository = asnRegistryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAsnRegistryCommand request, CancellationToken ct)
    {
        // Check if ASN number already exists
        bool asnExists = await _asnRegistryRepository.ExistsAsync(
            asn => asn.Number == request.Number, ct);

        if (asnExists)
        {
            throw new ConflictException($"ASN Registry with number '{request.Number}' already exists");
        }

        var asnRegistry = new AsnRegistry
        {
            Number = request.Number,
            Description = request.Description
        };

        await _asnRegistryRepository.AddAsync(asnRegistry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return asnRegistry.Id;
    }
}
