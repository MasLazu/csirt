using Mapster;
using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.Protocols.Commands.CreateProtocol;

public class CreateProtocolCommandHandler : IRequestHandler<CreateProtocolCommand, Guid>
{
    private readonly IRepository<Protocol> _protocolRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProtocolCommandHandler(IRepository<Protocol> protocolRepository, IUnitOfWork unitOfWork)
    {
        _protocolRepository = protocolRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProtocolCommand request, CancellationToken ct)
    {
        string normalizedName = request.Name.Trim();
        if (await _protocolRepository.ExistsAsync(p => p.Name.ToLower() == normalizedName.ToLower(), ct))
        {
            throw new ConflictException($"Protocol with name '{request.Name}' already exists");
        }

        Protocol protocol = request.Adapt<Protocol>();
        protocol.Id = Guid.NewGuid();
        protocol.CreatedAt = DateTime.UtcNow;

        await _protocolRepository.AddAsync(protocol, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return protocol.Id;
    }
}
