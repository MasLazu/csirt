using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.AsnRegistries.Queries.GetAsnRegistry;

public class GetAsnRegistryQueryHandler : IRequestHandler<GetAsnRegistryQuery, AsnRegistryDto>
{
    private readonly IRepository<AsnRegistry> _asnRegistryRepository;

    public GetAsnRegistryQueryHandler(IRepository<AsnRegistry> asnRegistryRepository)
    {
        _asnRegistryRepository = asnRegistryRepository;
    }

    public async Task<AsnRegistryDto> Handle(GetAsnRegistryQuery request, CancellationToken ct)
    {
        AsnRegistry asnRegistry = await _asnRegistryRepository.FirstOrDefaultAsync(
            asn => asn.Id == request.Id, ct) ??
            throw new NotFoundException($"ASN Registry with ID {request.Id} not found");

        return asnRegistry.Adapt<AsnRegistryDto>();
    }
}
