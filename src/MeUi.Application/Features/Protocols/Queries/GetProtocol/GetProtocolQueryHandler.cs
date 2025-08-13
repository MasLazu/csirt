using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Application.Models;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Protocols.Queries.GetProtocol;

public class GetProtocolQueryHandler : IRequestHandler<GetProtocolQuery, ProtocolDto>
{
    private readonly IRepository<Protocol> _protocolRepository;

    public GetProtocolQueryHandler(IRepository<Protocol> protocolRepository)
    {
        _protocolRepository = protocolRepository;
    }

    public async Task<ProtocolDto> Handle(GetProtocolQuery request, CancellationToken ct)
    {
        Protocol? protocol = await _protocolRepository.GetByIdAsync(request.Id, ct);
        if (protocol is null)
        {
            throw new NotFoundException($"Protocol '{request.Id}' not found");
        }
        return protocol.Adapt<ProtocolDto>();
    }
}
