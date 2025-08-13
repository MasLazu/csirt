using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Protocols.Commands.DeleteProtocol;

public class DeleteProtocolCommandHandler : IRequestHandler<DeleteProtocolCommand, Guid>
{
    private readonly IRepository<Protocol> _protocolRepository;
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProtocolCommandHandler(IRepository<Protocol> protocolRepository, IRepository<ThreatEvent> threatEventRepository, IUnitOfWork unitOfWork)
    {
        _protocolRepository = protocolRepository;
        _threatEventRepository = threatEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(DeleteProtocolCommand request, CancellationToken ct)
    {
        Protocol? protocol = await _protocolRepository.GetByIdAsync(request.Id, ct);
        if (protocol is null)
        {
            throw new NotFoundException($"Protocol '{request.Id}' not found");
        }

        bool hasReferences = (await _threatEventRepository.ExistsAsync(te => te.ProtocolId == request.Id, ct));
        if (hasReferences)
        {
            throw new ConflictException("Cannot delete protocol that is referenced by threat events");
        }

        await _protocolRepository.DeleteAsync(protocol, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return request.Id;
    }
}
