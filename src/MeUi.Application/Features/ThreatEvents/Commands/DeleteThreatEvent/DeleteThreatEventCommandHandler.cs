using MediatR;
using MeUi.Application.Exceptions;
using MeUi.Application.Interfaces;
using MeUi.Domain.Entities;

namespace MeUi.Application.Features.ThreatEvents.Commands.DeleteThreatEvent;

public class DeleteThreatEventCommandHandler : IRequestHandler<DeleteThreatEventCommand>
{
    private readonly IRepository<ThreatEvent> _threatEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteThreatEventCommandHandler(IRepository<ThreatEvent> threatEventRepository, IUnitOfWork unitOfWork)
    {
        _threatEventRepository = threatEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteThreatEventCommand request, CancellationToken ct)
    {
        ThreatEvent threatEvent = await _threatEventRepository.GetByIdAsync(request.Id, ct) ??
            throw new NotFoundException($"ThreatEvent with ID {request.Id} not found");

        await _threatEventRepository.DeleteAsync(threatEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
