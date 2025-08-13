using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Protocols.Commands.UpdateProtocol;

public class UpdateProtocolCommandHandler : IRequestHandler<UpdateProtocolCommand, Guid>
{
    private readonly IRepository<Protocol> _protocolRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProtocolCommandHandler(IRepository<Protocol> protocolRepository, IUnitOfWork unitOfWork)
    {
        _protocolRepository = protocolRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(UpdateProtocolCommand request, CancellationToken ct)
    {
        Protocol? protocol = await _protocolRepository.GetByIdAsync(request.Id, ct);
        if (protocol is null)
        {
            throw new NotFoundException($"Protocol '{request.Id}' not found");
        }

        string normalizedName = request.Name.Trim();
        if (await _protocolRepository.ExistsAsync(p => p.Id != request.Id && p.Name.ToLower() == normalizedName.ToLower(), ct))
        {
            throw new ConflictException($"Protocol with name '{request.Name}' already exists");
        }

        protocol.Name = request.Name.Trim();
        protocol.UpdatedAt = DateTime.UtcNow;

        await _protocolRepository.UpdateAsync(protocol, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return protocol.Id;
    }
}
